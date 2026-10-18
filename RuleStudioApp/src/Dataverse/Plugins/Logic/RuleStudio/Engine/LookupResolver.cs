using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>
/// Lookup definitions (json-formats.md §1): the match criteria become one query on the
/// lookup table; wildcards are counted per row for the Most specific policy.
/// </summary>
internal sealed class LookupResolver(EngineContext context)
{
    private readonly EngineContext context = context;

    internal ResolvedValue Resolve(RuleRun run, CompiledLookup lookup)
    {
        var criteria = lookup.MatchCriteria?.Criteria ?? [];
        var query = new QueryExpression(lookup.Table) { ColumnSet = new ColumnSet(Columns(lookup).ToArray()), TopCount = EngineLimits.MaxRows + 1, NoLock = true };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        foreach (var criterion in criteria)
        {
            query.Criteria.AddFilter(Filter(run, lookup.Table, criterion));
        }

        if (!string.IsNullOrEmpty(lookup.SortColumn))
        {
            query.AddOrder(lookup.SortColumn, OrderType.Ascending);
        }

        var rows = RecordCache.Query(context, query);
        if (rows.Count > EngineLimits.MaxRows)
            throw new EngineException(EngineCodes.RowLimitExceeded, $"Lookup {lookup.Name} matches more than {EngineLimits.MaxRows} rows.");

        var trace = new LookupTrace { LookupDefinitionId = lookup.Id, MatchCount = rows.Count };
        return rows.Count == 0 ? NoMatch(lookup, trace) : Match(lookup, criteria, rows, trace);
    }

    private static HashSet<string> Columns(CompiledLookup lookup)
    {
        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { lookup.ReturnColumn };
        foreach (var criterion in lookup.MatchCriteria?.Criteria ?? [])
        {
            foreach (var column in new[] { criterion.Column, criterion.MinColumn, criterion.MaxColumn }.Where(c => !string.IsNullOrEmpty(c)))
            {
                columns.Add(column!);
            }
        }

        return columns;
    }

    private static ResolvedValue NoMatch(CompiledLookup lookup, LookupTrace trace)
    {
        switch (lookup.NoMatch)
        {
            case "useDefault":
                trace.Policy = "usedDefault";
                var value = Values.Normalize(lookup.DefaultValue);
                trace.Value = Values.ForTrace(value);
                return new ResolvedValue { Value = value, Lookup = trace };
            case "skip":
                trace.Policy = "skipped";
                return new ResolvedValue { Skipped = true, Lookup = trace };
            default:
                trace.Policy = "blocked";
                throw new EngineException(EngineCodes.LookupNoMatch, $"No row of lookup {lookup.Name} matches this record.", blocks: true) { Lookup = trace };
        }
    }

    private static ResolvedValue Match(CompiledLookup lookup, IList<MatchCriterion> criteria, IList<Entity> rows, LookupTrace trace)
    {
        if (rows.Count > 1 && string.Equals(lookup.MultiMatch, "error", StringComparison.Ordinal))
        {
            trace.Policy = "error";
            throw new EngineException(EngineCodes.LookupMultipleMatches, $"{rows.Count} rows of lookup {lookup.Name} match this record; it allows only one.") { Lookup = trace };
        }

        // Rows arrive in sort order. Most specific: fewest wildcards first, sort order breaks ties.
        var ranked = rows.Select((row, index) => (Row: row, Index: index, Wildcards: Wildcards(criteria, row)));
        if (!string.Equals(lookup.MultiMatch, "firstBySort", StringComparison.Ordinal))
        {
            ranked = ranked.OrderBy(r => r.Wildcards).ThenBy(r => r.Index);
        }

        var best = ranked.First();
        var value = best.Row.Attributes.TryGetValue(lookup.ReturnColumn, out var raw) ? Values.Normalize(raw) : null;
        trace.MatchedRowId = best.Row.Id;
        trace.Wildcards = best.Wildcards;
        trace.Value = Values.ForTrace(value);
        return new ResolvedValue { Value = value, Lookup = trace };
    }

    private static int Wildcards(IList<MatchCriterion> criteria, Entity row) =>
        criteria.Count(c => c.EmptyMatchesAll == true && (Empty(row, c.Column) || Empty(row, c.MinColumn) || Empty(row, c.MaxColumn)));

    private static bool Empty(Entity row, string? column) =>
        !string.IsNullOrEmpty(column) && (!row.Attributes.TryGetValue(column!, out var value) || Values.IsEmpty(Values.Normalize(value)));

    private FilterExpression Filter(RuleRun run, string table, MatchCriterion criterion)
    {
        var filter = new FilterExpression(LogicalOperator.And);
        switch (criterion.Operator)
        {
            case "equals":
                AddMatch(filter, table, criterion, [context.Sources.Simple(run, criterion.Value)]);
                break;
            case "in":
                AddMatch(filter, table, criterion, criterion.Values.Select(v => context.Sources.Simple(run, v)).ToList());
                break;
            case "range":
                AddRange(filter, table, criterion, context.Sources.Simple(run, criterion.Value));
                break;
            case "isEmpty":
                filter.AddCondition(criterion.Column, ConditionOperator.Null);
                break;
            default:
                throw new EngineException(EngineCodes.LookupNotFound, $"Lookup criterion operator {criterion.Operator} is not supported.");
        }

        return filter;
    }

    // row[column] is one of the values, or empty when the criterion is a wildcard.
    private void AddMatch(FilterExpression filter, string table, MatchCriterion criterion, List<object?> values)
    {
        var column = context.Metadata.Column(table, criterion.Column);
        var any = filter.AddFilter(LogicalOperator.Or);
        var present = values.Where(v => !Values.IsEmpty(v)).Select(v => Coerce.ToQuery(v, column)).ToArray();
        if (present.Length > 0)
        {
            any.AddCondition(criterion.Column, ConditionOperator.In, present);
        }

        if (criterion.EmptyMatchesAll == true || present.Length < values.Count)
        {
            any.AddCondition(criterion.Column, ConditionOperator.Null);
        }
    }

    // row[minColumn] ≤ value < row[maxColumn]; an empty bound is open.
    private void AddRange(FilterExpression filter, string table, MatchCriterion criterion, object? value)
    {
        foreach (var (bound, op) in new[] { (criterion.MinColumn, ConditionOperator.LessEqual), (criterion.MaxColumn, ConditionOperator.GreaterThan) })
        {
            if (string.IsNullOrEmpty(bound))
                continue;

            var side = filter.AddFilter(LogicalOperator.Or);
            side.AddCondition(bound, ConditionOperator.Null);
            if (!Values.IsEmpty(value))
            {
                side.AddCondition(bound, op, Coerce.ToQuery(value, context.Metadata.Column(table, bound)));
            }
        }
    }
}
