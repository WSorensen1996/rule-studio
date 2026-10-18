using System.Globalization;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>
/// Plain values the engine compares: null, string, decimal (every number, money and choice), bool,
/// DateTime, Guid, EntityReference and lists of these. Dataverse and JSON values are normalized to
/// them on read; set-field values are converted back to the column's SDK type on write.
/// </summary>
internal static class Values
{
    // before/after values longer than this are cut in the trace (json-formats.md, Decisions 2).
    internal const int MaxTraceText = 1000;

    internal static object? Normalize(object? raw) => raw switch
    {
        null => null,
        AliasedValue aliased => Normalize(aliased.Value),
        JValue value => Normalize(value.Value),
        JObject reference => Reference(reference),
        JArray array => array.Select(Normalize).ToList(),
        OptionSetValue option => (decimal)option.Value,
        OptionSetValueCollection options => options.Select(o => (object?)(decimal)o.Value).ToList(),
        Money money => money.Value,
        EntityReference reference => reference,
        string text => text,
        bool flag => flag,
        DateTime date => date.Kind == DateTimeKind.Local ? date.ToUniversalTime() : date,
        Guid id => id,
        int or long or short or byte or decimal or double or float => Convert.ToDecimal(raw, CultureInfo.InvariantCulture),
        RecordReference record => new EntityReference(record.Table, record.Id) { Name = record.Name },
        IEnumerable<object?> list => list.Select(Normalize).ToList(),
        _ => Convert.ToString(raw, CultureInfo.InvariantCulture),
    };

    internal static bool IsEmpty(object? value) =>
        value == null || (value is string text && text.Length == 0) || (value is IList<object?> list && list.Count == 0);

    internal static bool AreEqual(object? left, object? right)
    {
        if (IsEmpty(left) || IsEmpty(right))
            return IsEmpty(left) && IsEmpty(right);

        // A multi-select choice (or any list) is a set: equal when both hold the same values, in any order.
        if (left is IList<object?> || right is IList<object?>)
        {
            var a = AsList(left);
            var b = AsList(right);
            return a.All(x => b.Any(y => AreEqual(x, y))) && b.All(y => a.Any(x => AreEqual(x, y)));
        }

        if (Id(left) is { } leftId && Id(right) is { } rightId)
            return leftId == rightId;

        if (left is bool || right is bool)
            return Bool(left) is { } a && Bool(right) is { } b && a == b;

        var order = Compare(left, right);
        return order.HasValue ? order.Value == 0 : string.Equals(Text(left), Text(right), StringComparison.OrdinalIgnoreCase);
    }

    // Ordering of two values of a comparable kind (number, date, text); null when they cannot be compared.
    // Two texts always compare as text ("007" is not "7"); a text is read as a number only against a number.
    // A date without a time ("2026-01-01") compares calendar days, in UTC.
    internal static int? Compare(object? left, object? right)
    {
        if (IsEmpty(left) || IsEmpty(right) || left is IList<object?> || right is IList<object?>)
            return null;

        if (left is string l && right is string r)
            return string.Compare(l, r, StringComparison.OrdinalIgnoreCase);

        if (left is DateTime || right is DateTime)
        {
            if (Date(left) is not { } x || Date(right) is not { } y)
                return null;

            return IsDay(left) || IsDay(right) ? x.Date.CompareTo(y.Date) : x.CompareTo(y);
        }

        if (Number(left) is { } a && Number(right) is { } b)
            return a.CompareTo(b);

        return null;
    }

    // The values of a list, or the single value as a list of one.
    internal static IList<object?> AsList(object? value) => value as IList<object?> ?? (IsEmpty(value) ? [] : [value]);

    // A date text without a time part, e.g. "2026-01-01".
    private static bool IsDay(object? value) =>
        value is string text && DateTime.TryParseExact(text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

    internal static string Text(object? value) => value switch
    {
        null => string.Empty,
        EntityReference reference => reference.Name ?? reference.Id.ToString(),
        DateTime date => date.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
        IList<object?> list => string.Join(", ", list.Select(Text)),
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
    };

    internal static decimal? Number(object? value) => value switch
    {
        decimal number => number,
        bool flag => flag ? 1 : 0,
        string text when decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) => parsed,
        _ => null,
    };

    // Wraps a value for the trace document: references as { table, id, name }, long text cut.
    internal static object? ForTrace(object? value) => value switch
    {
        EntityReference reference => new RecordReference { Table = reference.LogicalName, Id = reference.Id, Name = reference.Name },
        Guid id => id.ToString(),
        string text when text.Length > MaxTraceText => text.Remove(MaxTraceText) + "…",
        IList<object?> list => list.Select(ForTrace).ToList(),
        _ => value,
    };

    // A value for NCalc: references become their id text, everything else stays as normalized.
    internal static object? ForExpression(object? value) => value switch
    {
        EntityReference reference => reference.Id.ToString(),
        Guid id => id.ToString(),
        _ => value,
    };

    private static Guid? Id(object? value) => value switch
    {
        EntityReference reference => reference.Id,
        Guid id => id,
        string text when Guid.TryParse(text, out var parsed) => parsed,
        _ => null,
    };

    private static bool? Bool(object? value) => value switch
    {
        bool flag => flag,
        decimal number => number != 0,
        string text when bool.TryParse(text, out var parsed) => parsed,
        _ => null,
    };

    private static DateTime? Date(object? value) => value switch
    {
        DateTime date => date.Kind == DateTimeKind.Local ? date.ToUniversalTime() : date,
        string text when DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var parsed) => parsed,
        _ => null,
    };

    private static object? Reference(JObject value)
    {
        var id = value.Value<string>("id");
        if (!Guid.TryParse(id, out var guid))
            return value.ToString(Newtonsoft.Json.Formatting.None);

        return new EntityReference(value.Value<string>("table") ?? string.Empty, guid) { Name = value.Value<string>("name") };
    }
}
