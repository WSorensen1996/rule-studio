using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>A rule failed while it was evaluated. <see cref="Code"/> goes into the trace's error.</summary>
public sealed class EngineException : Exception
{
    public EngineException()
    {
    }

    public EngineException(string message)
        : base(message)
    {
    }

    public EngineException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public EngineException(string code, string message, bool blocks = false)
        : base(message)
    {
        Code = code;
        Blocks = blocks;
    }

    public string Code { get; } = EngineCodes.RuleFailed;

    // A lookup's Block policy: the save is blocked whatever the rule's failure policy is.
    public bool Blocks { get; }

    // The lookup evaluation that failed, so the trace can show it.
    public LookupTrace? Lookup { get; set; }
}
