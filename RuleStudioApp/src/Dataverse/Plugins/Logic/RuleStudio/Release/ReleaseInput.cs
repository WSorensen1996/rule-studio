using RuleStudio.SharedContext;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>Everything a release check needs, loaded once.</summary>
internal sealed class ReleaseInput
{
    internal List<RuleDraft> Drafts { get; } = [];

    // Selected rules that have no draft.
    internal List<rs_Rule> WithoutDraft { get; } = [];

    internal Dictionary<Guid, rs_LookupDefinition> Lookups { get; } = [];

    internal Dictionary<Guid, rs_CustomFunction> Functions { get; } = [];

    // Names and Custom API names of every enabled custom function: what an expression may call.
    internal HashSet<string> FunctionNames { get; } = new(StringComparer.OrdinalIgnoreCase);
}
