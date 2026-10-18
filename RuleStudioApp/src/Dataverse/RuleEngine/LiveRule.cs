using SharedDomain.RuleStudio.Formats;

namespace RuleStudio.Engine;

/// <summary>A published rule version the engine runs.</summary>
public sealed class LiveRule
{
    public LiveRule(CompiledRule rule)
    {
        Rule = rule;
    }

    public CompiledRule Rule { get; }

    // The rule's own level, else Errors only.
    public string LogLevel => Rule.Runtime.LogLevel ?? EngineSettings.DefaultLogLevel;

    public bool RunAsSystem => string.Equals(Rule.Runtime.RunAs, "system", StringComparison.Ordinal);
}
