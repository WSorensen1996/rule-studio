namespace RuleStudio.Engine;

/// <summary>What one chain run decided: the rules in order, the field changes and the block message.</summary>
public sealed class ChainOutcome
{
    public IList<RuleRunRecord> Runs { get; } = new List<RuleRunRecord>();

    // Final value per column; a later rule overwrites an earlier one.
    public IDictionary<string, FieldChange> Changes { get; } = new Dictionary<string, FieldChange>(StringComparer.OrdinalIgnoreCase);

    // Set when the save must be blocked (before save only).
    public string? BlockMessage { get; set; }
}
