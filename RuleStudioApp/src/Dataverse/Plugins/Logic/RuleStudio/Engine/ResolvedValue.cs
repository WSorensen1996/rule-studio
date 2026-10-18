using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>A value source's value, with the lookup or function evaluation that produced it.</summary>
internal sealed class ResolvedValue
{
    internal object? Value { get; init; }

    // The lookup's Skip policy applied: the action is skipped, the condition is false.
    internal bool Skipped { get; init; }

    internal LookupTrace? Lookup { get; init; }

    internal FunctionTrace? Function { get; init; }

    // Throws when the function reported success = false; call after the trace is attached.
    internal void ThrowIfFailed()
    {
        if (Function is { Result.Success: false } function)
            throw new EngineException(function.TimedOut ? EngineCodes.FunctionTimeout : EngineCodes.FunctionFailed, $"{function.CustomApiName} failed: {function.Result.Message ?? "no message"}");
    }
}
