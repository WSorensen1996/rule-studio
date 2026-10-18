namespace DataverseLogic.RuleStudio.Simulation;

/// <summary>The inputs of rs_SimulateRule (docs/custom-apis.md).</summary>
public sealed class SimulationRequest
{
    // JSON array of rs_rule ids; or null to simulate the chain of Table, Event and Stage.
    public string? RuleIds { get; init; }

    public string? Table { get; init; }

    // create/update/delete or the rs_event choice value.
    public string? Event { get; init; }

    // beforeSave/afterSave or the rs_stage choice value.
    public string? Stage { get; init; }

    // Evaluate each rule's draft where it has one (default), else only published versions.
    public bool UseDrafts { get; init; } = true;

    public Guid? RecordId { get; init; }

    // JSON object: column → value, applied over the record. For Update these are the changed columns.
    public string? SampleValues { get; init; }

    // Also mock functions without side effects (default). Side-effect functions are always mocked.
    public bool MockFunctions { get; init; } = true;

    // Test only this lookup definition against Table's record (RecordId and/or SampleValues).
    public Guid? LookupDefinitionId { get; init; }
}
