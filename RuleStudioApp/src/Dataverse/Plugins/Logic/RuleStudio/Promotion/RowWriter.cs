using DataverseLogic.RuleStudio.Release;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Promotion;

/// <summary>
/// The reverse of <see cref="SnapshotCompiler"/>: writes a compiled rule's condition tree, actions
/// and input mappings as authoring rows of a draft version, so it publishes back to the same snapshot.
/// </summary>
internal sealed class RowWriter(IDataverseAccessObject dao)
{
    private readonly IDataverseAccessObject dao = dao;

    internal void Write(CompiledRule rule, Guid versionId)
    {
        var version = new EntityReference(rs_RuleVersion.EntityLogicalName, versionId);
        WriteGroup(rule.Conditions, version, null, 0);

        // Orders are 0-based, as the rule editor writes them.
        foreach (var (branch, actions) in new[] { (rs_action_rs_branch.Then, rule.Then), (rs_action_rs_branch.Else, rule.Else) })
        {
            for (var i = 0; i < actions.Count; i++)
            {
                WriteAction(actions[i], version, branch, i);
            }
        }
    }

    // The row columns that hold a value source (json-formats.md "Value source").
    internal static SourceColumns Columns(ValueSource? source)
    {
        if (source == null)
            return new SourceColumns();

        var type = ChoiceValues.Parse<rs_valuesourcetype>(source.Type, ChoiceNames.ValueSourceType);
        return type switch
        {
            rs_valuesourcetype.Fieldpath => new SourceColumns { Type = type, Path = source.Path == null ? null : Json.Write(source.Path) },
            rs_valuesourcetype.Expression => new SourceColumns { Type = type, Value = source.Expression },
            rs_valuesourcetype.Lookup => new SourceColumns { Type = type, Lookup = Reference(rs_LookupDefinition.EntityLogicalName, source.LookupDefinitionId) },
            rs_valuesourcetype.Function => new SourceColumns { Type = type, Function = Reference(rs_CustomFunction.EntityLogicalName, source.CustomFunctionId) },
            _ => new SourceColumns { Type = rs_valuesourcetype.Fixed, Value = JsonConvert.SerializeObject(source.Value) },
        };
    }

    private static EntityReference? Reference(string table, Guid? id) => id is { } value ? new EntityReference(table, value) : null;

    private void WriteGroup(CompiledNode node, EntityReference version, EntityReference? parent, int order)
    {
        var id = dao.Create(new rs_ConditionGroup
        {
            rs_Name = parent == null ? "Root" : $"Group {order + 1}",
            rs_LogicalOperator = string.Equals(node.Operator, "or", StringComparison.Ordinal) ? rs_conditiongroup_rs_logicaloperator.OR : rs_conditiongroup_rs_logicaloperator.AND,
            rs_Order = order,
            rs_RuleVersionId = version,
            rs_ParentGroupId = parent,
        });
        var group = new EntityReference(rs_ConditionGroup.EntityLogicalName, id);
        var index = -1;
        foreach (var child in node.Children)
        {
            index++;
            if (string.Equals(child.Type, "group", StringComparison.Ordinal))
            {
                WriteGroup(child, version, group, index);
            }
            else
            {
                WriteCondition(child, group, index);
            }
        }
    }

    private void WriteCondition(CompiledNode node, EntityReference group, int order)
    {
        var source = Columns(node.Value);
        var id = dao.Create(new rs_Condition
        {
            rs_Name = $"Condition {order + 1}",
            rs_Order = order,
            rs_ConditionGroupId = group,
            rs_FieldPath = node.Path == null ? null : Json.Write(node.Path),
            rs_Aggregate = ChoiceValues.Parse<rs_condition_rs_aggregate>(node.Aggregate, ChoiceNames.Aggregate),
            rs_Operator = ChoiceValues.Parse<rs_condition_rs_operator>(node.Operator, ChoiceNames.Operator),
            rs_ValueSourceType = source.Type,
            rs_Value = source.Value,
            rs_ValuePath = source.Path,
            rs_LookupDefinitionId = source.Lookup,
            rs_CustomFunctionId = source.Function,
        });
        WriteInputs(node.Inputs, new EntityReference(rs_Condition.EntityLogicalName, id), null);
    }

    private void WriteAction(CompiledAction compiled, EntityReference version, rs_action_rs_branch branch, int order)
    {
        var source = Columns(compiled.Value);
        var id = dao.Create(new rs_Action
        {
            rs_Name = $"Action {order + 1}",
            rs_Order = order,
            rs_RuleVersionId = version,
            rs_Branch = branch,
            rs_ActionType = ChoiceValues.Parse<rs_action_rs_actiontype>(compiled.Type, ChoiceNames.ActionType),
            rs_TargetField = compiled.TargetField,
            rs_Message = compiled.Message,
            rs_MessageLevel = ChoiceValues.Parse<rs_messagelevel>(compiled.MessageLevel, ChoiceNames.MessageLevel),
            rs_ValueSourceType = source.Type,
            rs_Value = source.Value,
            rs_ValuePath = source.Path,
            rs_LookupDefinitionId = source.Lookup,
            rs_CustomFunctionId = source.Function,
        });
        WriteInputs(compiled.Inputs, null, new EntityReference(rs_Action.EntityLogicalName, id));
    }

    private void WriteInputs(IList<CompiledInput> inputs, EntityReference? condition, EntityReference? action)
    {
        foreach (var input in inputs)
        {
            var source = Columns(input.Value);
            dao.Create(new rs_ActionInputMapping
            {
                rs_Name = input.Name,
                rs_ParameterName = input.Name,
                rs_ValueSourceType = source.Type,
                rs_Value = source.Value,
                rs_ValuePath = source.Path,
                rs_ConditionId = condition,
                rs_ActionId = action,
            });
        }
    }
}
