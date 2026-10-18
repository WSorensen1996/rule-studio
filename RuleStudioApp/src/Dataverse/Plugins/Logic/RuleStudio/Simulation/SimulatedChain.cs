using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Simulation;

/// <summary>The compiled rules a test run evaluates, and where each came from.</summary>
internal sealed class SimulatedChain
{
    internal string Table { get; set; } = string.Empty;

    internal string Event { get; set; } = string.Empty;

    internal string Stage { get; set; } = string.Empty;

    internal List<CompiledRule> Rules { get; } = [];

    // Rules evaluated from their draft rather than their published version.
    internal HashSet<Guid> Drafts { get; } = [];

    internal Dictionary<Guid, string> Names { get; } = [];

    // Rules in the chain that do not run, with their skip reason.
    internal Dictionary<Guid, string> Skip { get; } = [];
}
