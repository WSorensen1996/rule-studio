using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Steps;

/// <summary>
/// The pure part of step reconciliation: which engine steps the published rules need, and what
/// must change so the registered steps match them exactly (docs/engine.md).
/// </summary>
public static class StepPlanner
{
    public const int PreOperation = 20;
    public const int PostOperation = 40;
    public const int Synchronous = 0;
    public const int Asynchronous = 1;

    // One step per table, message and stage. Update steps filter on the union of the filtered
    // columns, or on nothing when any rule of the chain has none.
    public static IReadOnlyList<DesiredStep> Desired(IEnumerable<RuleTrigger> triggers)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(triggers, nameof(triggers));
        return triggers
            .Select(t => (Trigger: t, Message: Message(t.Event), Stage: Stage(t.Stage), Table: t.Table.Trim().ToLowerInvariant()))
            .Where(t => t.Message != null && t.Stage != 0 && t.Table.Length > 0)
            .GroupBy(t => (t.Table, t.Message, t.Stage))
            .Select(g => new DesiredStep(g.Key.Table, g.Key.Message!, g.Key.Stage, Filter(g.Key.Message!, g.Select(t => t.Trigger))))
            .OrderBy(s => s.Key, StringComparer.Ordinal)
            .ToList();
    }

    // Matches existing steps to desired ones by table, message, stage and mode.
    public static StepPlan Plan(IEnumerable<DesiredStep> desired, IEnumerable<ExistingStep> existing)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(desired, nameof(desired));
        ArgumentNullExceptionHelper.ThrowIfNull(existing, nameof(existing));
        var plan = new StepPlan();
        var registered = existing.GroupBy(e => e.Key, StringComparer.Ordinal).ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);
        var wanted = new HashSet<string>(StringComparer.Ordinal);
        foreach (var step in desired)
        {
            wanted.Add(step.Key);
            if (!registered.TryGetValue(step.Key, out var matches))
            {
                plan.Create.Add(step);
                continue;
            }

            Compare(plan, matches[0], step);
            foreach (var duplicate in matches.Skip(1))
            {
                plan.Delete.Add(duplicate);
            }
        }

        foreach (var unused in registered.Where(r => !wanted.Contains(r.Key)).SelectMany(r => r.Value))
        {
            plan.Delete.Add(unused);
        }

        return plan;
    }

    // Comma-separated, lowercase, sorted and distinct, so stored values compare reliably.
    public static string Normalize(string? attributes) =>
        string.Join(",", (attributes ?? string.Empty)
            .Split(',')
            .Select(a => a.Trim().ToLowerInvariant())
            .Where(a => a.Length > 0)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(a => a, StringComparer.Ordinal));

    internal static string Key(string table, string message, int stage, int mode) =>
        $"{table.ToLowerInvariant()}|{message}|{stage}|{mode}";

    private static void Compare(StepPlan plan, ExistingStep existing, DesiredStep step)
    {
        var change = new StepChange(existing, step)
        {
            UpdateStep = !string.Equals(existing.Name, step.Name, StringComparison.Ordinal)
                || !string.Equals(Normalize(existing.FilteringAttributes), step.FilteringAttributes, StringComparison.Ordinal)
                || (step.Mode == Asynchronous && !existing.AsyncAutoDelete),
            AddPreImage = step.PreImage && !existing.HasPreImage,
        };
        if (change.UpdateStep || change.AddPreImage)
        {
            plan.Change.Add(change);
        }
        else
        {
            plan.Unchanged++;
        }
    }

    private static string Filter(string message, IEnumerable<RuleTrigger> triggers)
    {
        if (!string.Equals(message, "Update", StringComparison.Ordinal))
            return string.Empty;

        var all = triggers.ToList();
        if (all.Exists(t => t.FilteredColumns.Count == 0))
            return string.Empty;

        return Normalize(string.Join(",", all.SelectMany(t => t.FilteredColumns)));
    }

    private static string? Message(string eventName) => eventName switch
    {
        "create" => "Create",
        "update" => "Update",
        "delete" => "Delete",
        _ => null,
    };

    private static int Stage(string stage) => stage switch
    {
        "beforeSave" => PreOperation,
        "afterSave" => PostOperation,
        _ => 0,
    };
}
