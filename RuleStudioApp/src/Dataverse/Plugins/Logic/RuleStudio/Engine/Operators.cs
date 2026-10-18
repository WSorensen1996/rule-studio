using System.Globalization;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>The condition operators (json-formats.md, condition operators as in rs_trace).</summary>
internal static class Operators
{
    // Operators that compare against no value.
    internal static bool NeedsValue(string op) => op is not ("isEmpty" or "isNotEmpty" or "changed");

    internal static bool Test(string op, object? actual, object? expected) => op switch
    {
        "equals" => Values.AreEqual(actual, expected),
        "notEquals" => !Values.AreEqual(actual, expected),
        "greaterThan" => Values.Compare(actual, expected) > 0,
        "greaterOrEqual" => Values.Compare(actual, expected) >= 0,
        "lessThan" => Values.Compare(actual, expected) < 0,
        "lessOrEqual" => Values.Compare(actual, expected) <= 0,
        "contains" => Contains(actual, expected),
        "startsWith" => actual is not IList<object?> && !Values.IsEmpty(actual) && Values.Text(actual).StartsWith(Values.Text(expected), StringComparison.OrdinalIgnoreCase),
        "isEmpty" => Values.IsEmpty(actual),
        "isNotEmpty" => !Values.IsEmpty(actual),
        "inList" => actual is IList<object?> selected
            ? selected.Any(s => Values.AsList(expected).Any(v => Values.AreEqual(s, v)))
            : List(expected).Any(v => Values.AreEqual(actual, v)),
        "between" => Between(actual, List(expected)),
        _ => throw new EngineException(EngineCodes.OperatorNotSupported, $"Operator {op} is not supported."),
    };

    private static IList<object?> List(object? expected) => expected as IList<object?> ?? [expected];

    // Text: case-insensitive substring. A multi-select choice: every given option is selected.
    private static bool Contains(object? actual, object? expected)
    {
        if (Values.IsEmpty(actual))
            return false;

        if (actual is IList<object?> selected)
        {
            var wanted = Values.AsList(expected);
            return wanted.Count > 0 && wanted.All(w => selected.Any(s => Values.AreEqual(s, w)));
        }

        return CultureInfo.InvariantCulture.CompareInfo.IndexOf(Values.Text(actual), Values.Text(expected), CompareOptions.OrdinalIgnoreCase) >= 0;
    }

    // Inclusive at both ends; an empty bound is open.
    private static bool Between(object? actual, IList<object?> bounds)
    {
        if (Values.IsEmpty(actual) || bounds.Count != 2)
            return false;

        var low = Values.IsEmpty(bounds[0]) || Values.Compare(actual, bounds[0]) >= 0;
        var high = Values.IsEmpty(bounds[1]) || Values.Compare(actual, bounds[1]) <= 0;
        return low && high;
    }
}
