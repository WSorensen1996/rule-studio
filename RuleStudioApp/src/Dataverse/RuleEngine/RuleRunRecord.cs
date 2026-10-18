using SharedDomain.RuleStudio.Formats;

namespace RuleStudio.Engine;

/// <summary>One rule of a chain run: its trace and what the execution log needs besides it.</summary>
public sealed class RuleRunRecord
{
    public RuleRunRecord(LiveRule rule, ExecutionTrace trace, int attempts)
    {
        Rule = rule;
        Trace = trace;
        Attempts = attempts;
    }

    public LiveRule Rule { get; }

    public ExecutionTrace Trace { get; }

    // Attempts including retries; 0 for a rule that did not run (skipped).
    public int Attempts { get; }

    // After save: the update of the triggering record with this rule's changes failed.
    public string? ApplyError { get; set; }
}
