using System.Xml.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>
/// Reads field paths (json-formats.md "Field path"). Parent hops read one related record each;
/// a child hop turns the path into a set, read with one FetchXML query (aggregate where possible).
/// </summary>
internal sealed class PathReader(EngineContext context)
{
    private const string ValueAlias = "rsvalue";

    private readonly EngineContext context = context;

    // The value at the end of a path without child hops.
    internal object? Read(RuleRun run, FieldPath path)
    {
        CheckHops(path);
        if (path.Hops.Any(h => IsChild(h)))
            throw new EngineException(EngineCodes.ChildPathWithoutAggregate, $"{Describe(path)} reads child records, so it needs an aggregate.");

        if (path.Hops.Count == 0)
            return run.Value(path.Column);

        var (table, id) = Follow(run, path.Hops, path.Hops.Count);
        return id == null ? null : context.Records.Read(table, id.Value, path.Column);
    }

    // The values of the end column across every child row of a path with child hops.
    internal IList<object?> ChildValues(RuleRun run, FieldPath path)
    {
        var start = Start(run, path);
        if (start == null)
            return [];

        var rows = RecordCache.Query(context, new FetchExpression(Fetch(path, start.Value, null)));
        if (rows.Count > EngineLimits.MaxRows)
            throw new EngineException(EngineCodes.RowLimitExceeded, $"{Describe(path)} matches more than {EngineLimits.MaxRows} child rows.");

        // Non-aggregate results name a linked column <link alias>.<column>.
        var last = path.Hops.Count - 1;
        var key = last > IndexOfChild(path) ? $"rshop{last}.{path.Column}" : path.Column;
        return rows.Select(r => r.Attributes.TryGetValue(key, out var value) ? Values.Normalize(value) : null).ToList();
    }

    // count, sum, min, max or average of the end column, computed by Dataverse.
    internal object? Aggregate(RuleRun run, FieldPath path, string aggregate)
    {
        var function = aggregate switch
        {
            "count" => "countcolumn",
            "average" => "avg",
            "sum" or "min" or "max" => aggregate,
            _ => throw new EngineException(EngineCodes.OperatorNotSupported, $"Aggregate {aggregate} is not supported."),
        };
        var start = Start(run, path);
        if (start == null)
            return function == "countcolumn" ? 0m : null;

        var row = RecordCache.Query(context, new FetchExpression(Fetch(path, start.Value, function))).FirstOrDefault();
        var value = row != null && row.Attributes.TryGetValue(ValueAlias, out var result) ? Values.Normalize(result) : null;
        return value ?? (function == "countcolumn" ? 0m : null);
    }

    internal static string Describe(FieldPath path) =>
        string.Join(" → ", new[] { path.Table }.Concat(path.Hops.Select(h => h.Table)).Concat([path.Column]));

    private static bool IsChild(FieldPathHop hop) => string.Equals(hop.Kind, "child", StringComparison.Ordinal);

    private static void CheckHops(FieldPath path)
    {
        if (string.IsNullOrWhiteSpace(path.Column))
            throw new EngineException(EngineCodes.FieldNotFound, "A field path has no column.");

        if (path.Hops.Count > EngineLimits.MaxHops)
            throw new EngineException(EngineCodes.HopLimitExceeded, $"{Describe(path)} follows {path.Hops.Count} related tables; at most {EngineLimits.MaxHops} are allowed.");
    }

    // Follows the first count hops (all parent hops) and returns the record reached, or a null id.
    private (string Table, Guid? Id) Follow(RuleRun run, IList<FieldPathHop> hops, int count)
    {
        if (count == 0)
            return (context.Record.LogicalName, context.Record.Id == Guid.Empty ? null : context.Record.Id);

        var reference = run.Value(hops[0].Attribute) as EntityReference;
        for (var i = 0; i < count; i++)
        {
            var hop = hops[i];
            if (reference == null || !string.Equals(reference.LogicalName, hop.Table, StringComparison.OrdinalIgnoreCase))
                return (hop.Table, null);

            if (i == count - 1)
                return (hop.Table, reference.Id);

            reference = context.Records.Read(hop.Table, reference.Id, hops[i + 1].Attribute) as EntityReference;
        }

        return (hops[count - 1].Table, null);
    }

    // The record the first child hop starts from; null when there is none (e.g. an unsaved record).
    private Guid? Start(RuleRun run, FieldPath path)
    {
        CheckHops(path);
        var first = IndexOfChild(path);
        if (first < 0)
            throw new EngineException(EngineCodes.OperatorNotSupported, $"{Describe(path)} has no child hop, so it cannot be aggregated.");

        return Follow(run, path.Hops, first).Id;
    }

    private static int IndexOfChild(FieldPath path)
    {
        for (var i = 0; i < path.Hops.Count; i++)
        {
            if (IsChild(path.Hops[i]))
                return i;
        }

        return -1;
    }

    // FetchXML from the first child table, linking the remaining hops; an aggregate is aliased.
    private string Fetch(FieldPath path, Guid startId, string? aggregate)
    {
        var first = IndexOfChild(path);
        var hop = path.Hops[first];
        var root = new XElement("entity", new XAttribute("name", hop.Table));
        var current = root;
        var table = hop.Table;
        for (var i = first + 1; i < path.Hops.Count; i++)
        {
            var next = path.Hops[i];
            var (from, to) = IsChild(next) ? (next.Attribute, context.PrimaryId(table)) : (context.PrimaryId(next.Table), next.Attribute);
            var link = new XElement(
                "link-entity",
                new XAttribute("name", next.Table),
                new XAttribute("from", from),
                new XAttribute("to", to),
                new XAttribute("link-type", "inner"),
                new XAttribute("alias", $"rshop{i}"));
            current.Add(link);
            current = link;
            table = next.Table;
        }

        var attribute = new XElement("attribute", new XAttribute("name", path.Column));
        if (aggregate != null)
        {
            attribute.Add(new XAttribute("alias", ValueAlias), new XAttribute("aggregate", aggregate));
        }

        current.Add(attribute);
        root.Add(new XElement(
            "filter",
            new XElement("condition", new XAttribute("attribute", hop.Attribute), new XAttribute("operator", "eq"), new XAttribute("value", startId))));
        var fetch = new XElement("fetch", new XAttribute("no-lock", "true"), root);
        if (aggregate == null)
        {
            fetch.Add(new XAttribute("top", EngineLimits.MaxRows + 1));
        }
        else
        {
            fetch.Add(new XAttribute("aggregate", "true"));
        }

        return fetch.ToString(SaveOptions.DisableFormatting);
    }
}
