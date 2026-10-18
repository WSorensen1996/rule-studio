using DataverseLogic;

namespace RuleStudio.Engine;

/// <summary>Which loaded rules run for one save.</summary>
public static class RuleSelection
{
    // The rules whose trigger matches, without Update rules none of whose filtered columns changed,
    // in chain order: priority, lowest first, then rule key.
    public static IReadOnlyList<LiveRule> Applicable(IEnumerable<LiveRule> rules, string table, PluginTrigger trigger, ISet<string>? changedColumns)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(rules, nameof(rules));
        ArgumentNullExceptionHelper.ThrowIfNull(trigger, nameof(trigger));
        return rules
            .Where(r => Matches(r, table, trigger) && ColumnsChanged(r, trigger, changedColumns))
            .OrderBy(r => r.Rule.Runtime.Priority)
            .ThenBy(r => r.Rule.RuleKey, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool Matches(LiveRule rule, string table, PluginTrigger trigger) =>
        string.Equals(rule.Rule.Trigger.Table, table, StringComparison.OrdinalIgnoreCase)
        && string.Equals(rule.Rule.Trigger.Event, trigger.Event, StringComparison.Ordinal)
        && string.Equals(rule.Rule.Trigger.Stage, trigger.Stage, StringComparison.Ordinal);

    private static bool ColumnsChanged(LiveRule rule, PluginTrigger trigger, ISet<string>? changedColumns)
    {
        var filtered = rule.Rule.Trigger.FilteredColumns;
        if (!string.Equals(trigger.Event, "update", StringComparison.Ordinal) || filtered.Count == 0 || changedColumns == null)
            return true;

        return filtered.Any(changedColumns.Contains);
    }
}
