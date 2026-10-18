using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>
/// Deep-copies a version's condition groups, conditions, actions and input mappings onto another
/// version, giving every row a new id and remapping the links between them. Groups that scope an
/// aggregate are not authored by the editor yet and are left out, as in <see cref="DraftLoader"/>.
/// </summary>
internal sealed class VersionCopier(IDataverseAccessObject dao)
{
    private readonly IDataverseAccessObject dao = dao;
    private readonly Dictionary<Guid, Guid> newIds = [];

    internal void CopyRows(Guid sourceVersionId, Guid targetVersionId)
    {
        var version = new EntityReference(rs_RuleVersion.EntityLogicalName, targetVersionId);
        var groups = dao.RetrieveMultipleByIds<rs_ConditionGroup>([sourceVersionId], "rs_ruleversionid")
            .Where(g => g.rs_AggregateScopeConditionId == null).ToList();
        CopyGroups(groups, version);

        var conditions = dao.RetrieveMultipleByIds<rs_Condition>(groups.Select(g => g.Id), "rs_conditiongroupid");
        foreach (var condition in conditions)
        {
            CopyCondition(condition);
        }

        var actions = dao.RetrieveMultipleByIds<rs_Action>([sourceVersionId], "rs_ruleversionid");
        foreach (var action in actions)
        {
            CopyAction(action, version);
        }

        var mappings = dao.RetrieveMultipleByIds<rs_ActionInputMapping>(conditions.Select(c => c.Id), "rs_conditionid")
            .Concat(dao.RetrieveMultipleByIds<rs_ActionInputMapping>(actions.Select(a => a.Id), "rs_actionid"));
        foreach (var mapping in mappings)
        {
            dao.Create(new rs_ActionInputMapping
            {
                rs_Name = mapping.rs_Name,
                rs_ParameterName = mapping.rs_ParameterName,
                rs_ValueSourceType = mapping.rs_ValueSourceType,
                rs_Value = mapping.rs_Value,
                rs_ValuePath = mapping.rs_ValuePath,
                rs_ConditionId = Remap(mapping.rs_ConditionId),
                rs_ActionId = Remap(mapping.rs_ActionId),
            });
        }
    }

    // Parents are created before their children, so every parent lookup points at a row that exists.
    private void CopyGroups(List<rs_ConditionGroup> groups, EntityReference version)
    {
        foreach (var group in groups)
        {
            newIds[group.Id] = Guid.NewGuid();
        }

        var pending = groups.ToList();
        while (pending.Count > 0)
        {
            var ready = pending.Where(g => g.rs_ParentGroupId == null || !pending.Exists(p => p.Id == g.rs_ParentGroupId.Id)).ToList();
            if (ready.Count == 0)
                throw new InvalidPluginExecutionException("The version's condition groups form a loop and cannot be copied.");

            foreach (var group in ready)
            {
                dao.Create(new rs_ConditionGroup(newIds[group.Id])
                {
                    rs_Name = group.rs_Name,
                    rs_LogicalOperator = group.rs_LogicalOperator,
                    rs_Order = group.rs_Order,
                    rs_RuleVersionId = version,
                    rs_ParentGroupId = Remap(group.rs_ParentGroupId),
                });
                pending.Remove(group);
            }
        }
    }

    private void CopyCondition(rs_Condition condition)
    {
        newIds[condition.Id] = dao.Create(new rs_Condition
        {
            rs_Name = condition.rs_Name,
            rs_Order = condition.rs_Order,
            rs_ConditionGroupId = Remap(condition.rs_ConditionGroupId),
            rs_FieldPath = condition.rs_FieldPath,
            rs_Aggregate = condition.rs_Aggregate,
            rs_Operator = condition.rs_Operator,
            rs_ValueSourceType = condition.rs_ValueSourceType,
            rs_Value = condition.rs_Value,
            rs_ValuePath = condition.rs_ValuePath,
            rs_LookupDefinitionId = condition.rs_LookupDefinitionId,
            rs_CustomFunctionId = condition.rs_CustomFunctionId,
        });
    }

    private void CopyAction(rs_Action action, EntityReference version)
    {
        newIds[action.Id] = dao.Create(new rs_Action
        {
            rs_Name = action.rs_Name,
            rs_Order = action.rs_Order,
            rs_RuleVersionId = version,
            rs_Branch = action.rs_Branch,
            rs_ActionType = action.rs_ActionType,
            rs_TargetField = action.rs_TargetField,
            rs_ValueSourceType = action.rs_ValueSourceType,
            rs_Value = action.rs_Value,
            rs_ValuePath = action.rs_ValuePath,
            rs_LookupDefinitionId = action.rs_LookupDefinitionId,
            rs_CustomFunctionId = action.rs_CustomFunctionId,
            rs_Message = action.rs_Message,
            rs_MessageLevel = action.rs_MessageLevel,
        });
    }

    // A link to a row outside the copy is dropped rather than left pointing into the old version.
    private EntityReference? Remap(EntityReference? reference)
    {
        if (reference == null || !newIds.TryGetValue(reference.Id, out var id))
            return null;

        return new EntityReference(reference.LogicalName, id);
    }
}
