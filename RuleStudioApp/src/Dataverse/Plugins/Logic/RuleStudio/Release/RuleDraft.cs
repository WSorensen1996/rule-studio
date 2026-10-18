using RuleStudio.SharedContext;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>A rule and the authoring rows of the version being checked.</summary>
internal sealed class RuleDraft
{
    internal RuleDraft(rs_Rule rule, rs_RuleVersion version)
    {
        Rule = rule;
        Version = version;
    }

    internal rs_Rule Rule { get; }

    internal rs_RuleVersion Version { get; }

    internal string RuleKey => Rule.rs_RuleKey ?? Rule.Id.ToString();

    internal List<rs_ConditionGroup> Groups { get; } = [];

    internal List<rs_Condition> Conditions { get; } = [];

    internal List<rs_Action> Actions { get; } = [];

    internal List<rs_ActionInputMapping> Mappings { get; } = [];

    internal IEnumerable<rs_ActionInputMapping> MappingsOf(Guid ownerId) =>
        Mappings.Where(m => m.rs_ConditionId?.Id == ownerId || m.rs_ActionId?.Id == ownerId);
}
