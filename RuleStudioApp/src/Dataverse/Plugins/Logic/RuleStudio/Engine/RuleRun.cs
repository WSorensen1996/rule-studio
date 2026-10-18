using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>One attempt at one rule. Its field changes are staged and only committed when it completes.</summary>
internal sealed class RuleRun
{
    internal RuleRun(EngineContext context, CompiledRule rule, ExecutionTrace trace)
    {
        Context = context;
        Rule = rule;
        Trace = trace;
    }

    internal EngineContext Context { get; }

    internal CompiledRule Rule { get; }

    internal ExecutionTrace Trace { get; }

    internal Dictionary<string, object?> Changes { get; } = new(StringComparer.OrdinalIgnoreCase);

    internal List<FunctionTrace> Functions { get; } = [];

    internal SortedSet<string> Mocked { get; } = new(StringComparer.OrdinalIgnoreCase);

    // A function that is not safe to retry was really called: this attempt must not be repeated.
    internal bool CalledUnsafe { get; set; }

    internal string? BlockMessage { get; set; }

    // A column of the triggering record as this rule sees it: its own changes first.
    internal object? Value(string column)
    {
        if (Changes.TryGetValue(column, out var changed))
            return Values.Normalize(changed);

        return Context.Record.Attributes.TryGetValue(column, out var value) ? Values.Normalize(value) : null;
    }
}
