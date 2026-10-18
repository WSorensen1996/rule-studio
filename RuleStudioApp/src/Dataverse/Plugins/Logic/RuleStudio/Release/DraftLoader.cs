using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>Loads the selected rules' drafts with all their authoring rows in a handful of queries.</summary>
internal sealed class DraftLoader(IDataverseAccessObject dao)
{
    private readonly IDataverseAccessObject dao = dao;

    internal ReleaseInput Load(IReadOnlyCollection<Guid> ruleIds)
    {
        var input = new ReleaseInput();
        var rules = dao.RetrieveMultipleByIds<rs_Rule>(ruleIds);
        var missing = ruleIds.Except(rules.Select(r => r.Id)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException($"Rule {missing[0]} no longer exists.");

        var versions = dao.RetrieveMultipleByIds<rs_RuleVersion>(
            rules.Where(r => r.rs_DraftVersionId != null).Select(r => r.rs_DraftVersionId!.Id))
            .ToDictionary(v => v.Id);
        foreach (var rule in rules.OrderBy(r => r.rs_RuleKey, StringComparer.OrdinalIgnoreCase))
        {
            if (rule.rs_DraftVersionId != null && versions.TryGetValue(rule.rs_DraftVersionId.Id, out var version))
            {
                input.Drafts.Add(new RuleDraft(rule, version));
            }
            else
            {
                input.WithoutDraft.Add(rule);
            }
        }

        LoadRows(input.Drafts);
        LoadReferences(input);
        return input;
    }

    private void LoadRows(List<RuleDraft> drafts)
    {
        var byVersion = drafts.ToDictionary(d => d.Version.Id);

        // Groups that scope an aggregate are not authored by the editor yet; they are left out everywhere.
        var groups = dao.RetrieveMultipleByIds<rs_ConditionGroup>(byVersion.Keys, "rs_ruleversionid")
            .Where(g => g.rs_AggregateScopeConditionId == null).ToList();
        foreach (var group in groups)
        {
            byVersion[group.rs_RuleVersionId!.Id].Groups.Add(group);
        }

        var draftOfGroup = groups.ToDictionary(g => g.Id, g => byVersion[g.rs_RuleVersionId!.Id]);
        var conditions = dao.RetrieveMultipleByIds<rs_Condition>(draftOfGroup.Keys, "rs_conditiongroupid");
        foreach (var condition in conditions)
        {
            draftOfGroup[condition.rs_ConditionGroupId!.Id].Conditions.Add(condition);
        }

        var actions = dao.RetrieveMultipleByIds<rs_Action>(byVersion.Keys, "rs_ruleversionid");
        foreach (var action in actions)
        {
            byVersion[action.rs_RuleVersionId!.Id].Actions.Add(action);
        }

        var draftOfOwner = conditions.ToDictionary(c => c.Id, c => draftOfGroup[c.rs_ConditionGroupId!.Id]);
        foreach (var action in actions)
        {
            draftOfOwner[action.Id] = byVersion[action.rs_RuleVersionId!.Id];
        }

        var mappings = dao.RetrieveMultipleByIds<rs_ActionInputMapping>(conditions.Select(c => c.Id), "rs_conditionid")
            .Concat(dao.RetrieveMultipleByIds<rs_ActionInputMapping>(actions.Select(a => a.Id), "rs_actionid"));
        foreach (var mapping in mappings)
        {
            draftOfOwner[(mapping.rs_ConditionId ?? mapping.rs_ActionId)!.Id].Mappings.Add(mapping);
        }
    }

    private void LoadReferences(ReleaseInput input)
    {
        var lookupIds = new HashSet<Guid>();
        var functionIds = new HashSet<Guid>();
        foreach (var draft in input.Drafts)
        {
            Collect(draft.Conditions.Select(c => (c.rs_LookupDefinitionId, c.rs_CustomFunctionId)), lookupIds, functionIds);
            Collect(draft.Actions.Select(a => (a.rs_LookupDefinitionId, a.rs_CustomFunctionId)), lookupIds, functionIds);
        }

        foreach (var lookup in dao.RetrieveMultipleByIds<rs_LookupDefinition>(lookupIds))
        {
            input.Lookups[lookup.Id] = lookup;
        }

        foreach (var function in dao.RetrieveMultipleByIds<rs_CustomFunction>(functionIds))
        {
            input.Functions[function.Id] = function;
        }

        var all = new Microsoft.Xrm.Sdk.Query.QueryExpression(rs_CustomFunction.EntityLogicalName) { ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("rs_name", "rs_customapiname", "rs_enabled"), TopCount = 5000 };
        all.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
        foreach (var function in dao.RetrieveList<rs_CustomFunction>(all).Where(f => f.rs_Enabled != false))
        {
            foreach (var name in new[] { function.rs_Name, function.rs_CustomApiName }.Where(n => !string.IsNullOrWhiteSpace(n)))
            {
                input.FunctionNames.Add(name!);
            }
        }
    }

    private static void Collect(
        IEnumerable<(Microsoft.Xrm.Sdk.EntityReference? Lookup, Microsoft.Xrm.Sdk.EntityReference? Function)> references,
        HashSet<Guid> lookupIds,
        HashSet<Guid> functionIds)
    {
        foreach (var (lookup, function) in references)
        {
            if (lookup != null)
            {
                lookupIds.Add(lookup.Id);
            }

            if (function != null)
            {
                functionIds.Add(function.Id);
            }
        }
    }
}
