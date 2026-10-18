using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>What a chain decided: one trace per rule and what the caller must apply to the save.</summary>
public sealed class EvaluationResult
{
    // One rs_trace document per rule, in chain order.
    public IList<ExecutionTrace> Traces { get; } = new List<ExecutionTrace>();

    // Columns set by setField actions of rules that completed, as SDK values for the target.
    public IDictionary<string, object?> Changes { get; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

    // Set when the save must be blocked: a blockSave action or a failure under the Block policy.
    public string? BlockMessage { get; set; }

    // Attempts per rule id, retries included, for rules that ran (rs_executionlog.rs_attempts).
    public IDictionary<Guid, int> Attempts { get; } = new Dictionary<Guid, int>();

    // Functions called (or mocked) by completed and failed rules, in call order.
    public IList<FunctionTrace> FunctionCalls { get; } = new List<FunctionTrace>();
}
