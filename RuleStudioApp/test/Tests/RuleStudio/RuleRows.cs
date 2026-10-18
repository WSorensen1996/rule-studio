using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace Tests.RuleStudio;

/// <summary>Writes rule drafts as authoring rows (what the rule editor saves), for XrmMockup tests.</summary>
public sealed class RuleRows(IDataverseAccessObject dao)
{
    private readonly IDataverseAccessObject dao = dao;

    public static string Path(string table, string column, params (string Kind, string Attribute, string Table)[] hops) =>
        JsonConvert.SerializeObject(new
        {
            table,
            hops = hops.Select(h => new { kind = h.Kind, attribute = h.Attribute, table = h.Table }),
            column,
        });

    public Guid NewRule(string key) =>
        dao.Create(new rs_Rule
        {
            rs_Name = "TEST – " + key,
            rs_RuleKey = key,
            rs_Enabled = true,
        });

    // A new draft version of the rule with a root group; returns the version id.
    public Guid Draft(Guid ruleId, string table, rs_event @event, rs_stage stage, Action<rs_RuleVersion>? settings = null)
    {
        var number = dao.RetrieveMultipleByIds<rs_RuleVersion>([ruleId], "rs_ruleid", v => v.rs_VersionNumber)
            .Select(v => v.rs_VersionNumber ?? 0).DefaultIfEmpty(0).Max() + 1;
        var version = new rs_RuleVersion
        {
            rs_Name = $"v{number}",
            rs_RuleId = new EntityReference(rs_Rule.EntityLogicalName, ruleId),
            rs_VersionNumber = number,
            rs_Status = rs_ruleversion_rs_status.Draft,
            rs_TargetTable = table,
            rs_Event = @event,
            rs_Stage = stage,
            rs_Priority = 100,
            rs_FailurePolicy = rs_ruleversion_rs_failurepolicy.Block,
            rs_RunAs = rs_ruleversion_rs_runas.Callinguser,
            rs_LogLevel = rs_loglevel.Fulltrace,
        };
        settings?.Invoke(version);
        var versionId = dao.Create(version);
        dao.Create(new rs_ConditionGroup
        {
            rs_Name = "Root",
            rs_Order = 0,
            rs_LogicalOperator = rs_conditiongroup_rs_logicaloperator.AND,
            rs_RuleVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, versionId),
        });
        dao.Update(new rs_Rule(ruleId) { rs_DraftVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, versionId) });
        return versionId;
    }

    public Guid Root(Guid versionId) =>
        dao.RetrieveMultipleByIds<rs_ConditionGroup>([versionId], "rs_ruleversionid").First(g => g.rs_ParentGroupId == null).Id;

    public Guid Condition(Guid versionId, string path, rs_condition_rs_operator op, Action<rs_Condition>? value = null, rs_condition_rs_aggregate? aggregate = null)
    {
        var condition = new rs_Condition
        {
            rs_Name = "Condition",
            rs_Order = 0,
            rs_ConditionGroupId = new EntityReference(rs_ConditionGroup.EntityLogicalName, Root(versionId)),
            rs_FieldPath = path,
            rs_Operator = op,
            rs_Aggregate = aggregate,
        };
        value?.Invoke(condition);
        return dao.Create(condition);
    }

    public Guid Action(Guid versionId, rs_action_rs_actiontype type, Action<rs_Action>? settings = null, rs_action_rs_branch branch = rs_action_rs_branch.Then)
    {
        var action = new rs_Action
        {
            rs_Name = "Action",
            rs_Order = 0,
            rs_RuleVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, versionId),
            rs_Branch = branch,
            rs_ActionType = type,
        };
        settings?.Invoke(action);
        return dao.Create(action);
    }

    public Guid SetField(Guid versionId, string field, object? value, rs_action_rs_branch branch = rs_action_rs_branch.Then)
    {
        void Settings(rs_Action a)
        {
            a.rs_TargetField = field;
            a.rs_ValueSourceType = rs_valuesourcetype.Fixed;
            a.rs_Value = JsonConvert.SerializeObject(value);
        }

        return Action(versionId, rs_action_rs_actiontype.Setfield, Settings, branch);
    }

    public void Input(Guid ownerId, bool ownerIsAction, string name, rs_valuesourcetype type, string? value = null, string? valuePath = null) =>
        dao.Create(new rs_ActionInputMapping
        {
            rs_Name = name,
            rs_ParameterName = name,
            rs_ValueSourceType = type,
            rs_Value = value,
            rs_ValuePath = valuePath,
            rs_ActionId = ownerIsAction ? new EntityReference(rs_Action.EntityLogicalName, ownerId) : null,
            rs_ConditionId = ownerIsAction ? null : new EntityReference(rs_Condition.EntityLogicalName, ownerId),
        });

    public Guid Function(string apiName, bool sideEffects, bool safeToRetry, string schema) =>
        dao.Create(new rs_CustomFunction
        {
            rs_Name = apiName,
            rs_CustomApiName = apiName,
            rs_Enabled = true,
            rs_HasSideEffects = sideEffects,
            rs_SafeToRetry = safeToRetry,
            rs_OutputType = rs_customfunction_rs_outputtype.Decimal,
            rs_InputSchema = schema,
        });

    public Guid Discount() => Function(
        "rs_SampleCalculateDiscount",
        sideEffects: false,
        safeToRetry: true,
        schema: "{\"version\":1,\"parameters\":[{\"name\":\"Revenue\",\"type\":\"decimal\",\"required\":true},{\"name\":\"Employees\",\"type\":\"number\",\"required\":true}]}");

    public Guid Stamp() => Function(
        "rs_SampleStampContacts",
        sideEffects: true,
        safeToRetry: false,
        schema: "{\"version\":1,\"parameters\":[{\"name\":\"AccountId\",\"type\":\"lookup\",\"table\":\"account\",\"required\":true},{\"name\":\"Text\",\"type\":\"text\",\"required\":true}]}");
}
