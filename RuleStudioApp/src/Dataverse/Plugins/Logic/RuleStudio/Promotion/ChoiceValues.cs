namespace DataverseLogic.RuleStudio.Promotion;

/// <summary>The reverse of <see cref="Release.ChoiceNames"/>: a JSON name back to its choice value.</summary>
internal static class ChoiceValues
{
    // The first choice value whose JSON name is name; null when there is none.
    internal static T? Parse<T>(string? name, Func<T?, string?> toName)
        where T : struct, Enum
    {
        if (string.IsNullOrEmpty(name))
            return null;

#pragma warning disable CA2263 // The generic overload does not exist on net462.
        return Enum.GetValues(typeof(T)).Cast<T>().Select(v => (T?)v).FirstOrDefault(v => string.Equals(toName(v), name, StringComparison.Ordinal));
#pragma warning restore CA2263
    }
}
