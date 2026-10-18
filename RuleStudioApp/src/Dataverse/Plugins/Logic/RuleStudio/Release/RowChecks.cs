using DataverseLogic.RuleStudio.Engine;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>Checks the trigger, the conditions and the actions of one rule draft.</summary>
internal static class RowChecks
{
    private static readonly AttributeTypeCode[] TextTypes = [AttributeTypeCode.String, AttributeTypeCode.Memo];

    private static readonly AttributeTypeCode[] OrderedTypes =
        [AttributeTypeCode.Integer, AttributeTypeCode.BigInt, AttributeTypeCode.Decimal, AttributeTypeCode.Double, AttributeTypeCode.Money, AttributeTypeCode.DateTime];

    // False when the trigger is broken, so the remaining checks would only repeat it.
    internal static bool CheckTrigger(CheckContext context)
    {
        var version = context.Draft.Version;
        var at = new Location("trigger", version.Id, "The trigger");
        if (string.IsNullOrWhiteSpace(version.rs_TargetTable) || version.rs_Event == null || version.rs_Stage == null)
        {
            context.Add(CheckContext.Error, ValidationCodes.TableNotFound, "The trigger needs a table, an event and a stage.", at);
            return false;
        }

        if (context.Metadata.Table(version.rs_TargetTable) == null)
        {
            context.Add(CheckContext.Error, ValidationCodes.TableNotFound, $"The rule targets table {version.rs_TargetTable}, which does not exist.", at);
            return false;
        }

        if (version.rs_Event != rs_event.Update)
            return true;

        foreach (var column in SnapshotCompiler.FilteredColumns(version))
        {
            if (context.Metadata.Column(version.rs_TargetTable, column) == null)
            {
                context.Add(CheckContext.Error, ValidationCodes.FieldNotFound, $"Filtered column {column} does not exist on {version.rs_TargetTable}.", at);
            }
        }

        return true;
    }

    internal static void CheckCondition(CheckContext context, rs_Condition condition)
    {
        var at = new Location("condition", condition.Id, $"Condition {condition.rs_Name ?? string.Empty}".TrimEnd());
        var column = PathChecks.Check(context, Json.Read<FieldPath>(condition.rs_FieldPath), at, out var child);
        var aggregate = condition.rs_Aggregate ?? rs_condition_rs_aggregate.None;
        if (child)
        {
            context.ChildQueries++;
        }

        if (child && aggregate == rs_condition_rs_aggregate.None)
        {
            context.Add(CheckContext.Error, ValidationCodes.ChildPathWithoutAggregate, $"{at.Label} reads child records, so it needs an aggregate such as Any or Count.", at);
        }

        var op = condition.rs_Operator;
        if (op == null)
        {
            context.Add(CheckContext.Error, ValidationCodes.OperatorNotAllowed, $"{at.Label} has no operator.", at);
            return;
        }

        if (op == rs_condition_rs_operator.Changed && context.Draft.Version.rs_Event != rs_event.Update)
        {
            context.Add(CheckContext.Error, ValidationCodes.OperatorNotAllowed, $"{at.Label} uses Changed, which only works on Update triggers.", at);
        }

        // Count, sum and average turn any column into a number.
        var numericAggregate = aggregate is rs_condition_rs_aggregate.Count or rs_condition_rs_aggregate.Sum or rs_condition_rs_aggregate.Average;
        if (column?.AttributeType != null && !numericAggregate)
        {
            CheckOperatorType(context, op.Value, column, at);
        }

        if (op is rs_condition_rs_operator.Isempty or rs_condition_rs_operator.Isnotempty or rs_condition_rs_operator.Changed)
            return;

        var source = Json.Source(condition.rs_ValueSourceType, condition.rs_Value, condition.rs_ValuePath, (condition.rs_LookupDefinitionId?.Id, condition.rs_CustomFunctionId?.Id));
        if (source == null)
        {
            context.Add(CheckContext.Error, ValidationCodes.TypeMismatch, $"{at.Label} has no value to compare with.", at);
        }
        else if (source.Type == "fixed")
        {
            CheckFixedShape(context, op.Value, (JToken)source.Value!, at);
            CheckFixedValue(context, op.Value, (JToken)source.Value!, numericAggregate ? null : column, at);
        }

        ValueChecks.Check(context, source, condition.Id, at);
    }

    internal static void CheckAction(CheckContext context, rs_Action action)
    {
        var branch = action.rs_Branch == rs_action_rs_branch.Else ? "Else" : "Then";
        var at = new Location("action", action.Id, $"{branch} action {(action.rs_Order ?? 0) + 1}");
        switch (action.rs_ActionType)
        {
            case rs_action_rs_actiontype.Setfield:
                if (context.Metadata.Column(context.Table, action.rs_TargetField) == null)
                {
                    context.Add(CheckContext.Error, ValidationCodes.FieldNotFound, $"{at.Label} sets {action.rs_TargetField ?? "no field"}, which does not exist on {context.Table}.", at);
                }

                CheckActionValue(context, action, at);
                break;
            case rs_action_rs_actiontype.Blocksave:
                if (context.Draft.Version.rs_Stage == rs_stage.Aftersave)
                {
                    context.Add(CheckContext.Error, ValidationCodes.BlockSaveAfterSave, $"{at.Label} blocks the save, which is only possible before save.", at);
                }

                break;
            case rs_action_rs_actiontype.Callfunction:
                CheckActionValue(context, action, at);
                break;
            case rs_action_rs_actiontype.Logmessage:
                break;
            default:
                context.Add(CheckContext.Error, ValidationCodes.TypeMismatch, $"{at.Label} has no action type.", at);
                break;
        }
    }

    private static void CheckActionValue(CheckContext context, rs_Action action, Location at)
    {
        var source = Json.Source(action.rs_ValueSourceType, action.rs_Value, action.rs_ValuePath, (action.rs_LookupDefinitionId?.Id, action.rs_CustomFunctionId?.Id));
        if (source == null || (action.rs_ActionType == rs_action_rs_actiontype.Callfunction && source.Type != "function"))
        {
            context.Add(CheckContext.Error, ValidationCodes.FunctionNotRegistered, $"{at.Label} has no value or function.", at);
        }
        else
        {
            ValueChecks.Check(context, source, action.Id, at);
        }
    }

    // Contains also works on a multi-select choice (every given option is selected); Starts with does not.
    private static void CheckOperatorType(CheckContext context, rs_condition_rs_operator op, AttributeMetadata column, Location at)
    {
        var type = column.AttributeType!.Value;
        var multiSelect = column is MultiSelectPicklistAttributeMetadata;
        var text = op is rs_condition_rs_operator.Startswith || (op is rs_condition_rs_operator.Contains && !multiSelect);
        var ordered = op is rs_condition_rs_operator.Greaterthan or rs_condition_rs_operator.Greaterorequal
            or rs_condition_rs_operator.Lessthan or rs_condition_rs_operator.Lessorequal or rs_condition_rs_operator.Between;
        if ((text && !TextTypes.Contains(type)) || (ordered && !OrderedTypes.Contains(type)))
        {
            var kind = multiSelect ? "multi-select choice" : type.ToString();
            context.Add(CheckContext.Error, ValidationCodes.TypeMismatch, $"{at.Label} uses {ChoiceNames.Operator(op)} on a {kind} column.", at);
        }
    }

    // A fixed value the operator can never use: reversed Between bounds, Contains or Starts with on
    // nothing, or a Yes/No column compared with text that is not a yes/no value.
    private static void CheckFixedValue(CheckContext context, rs_condition_rs_operator op, JToken value, AttributeMetadata? column, Location at)
    {
        if (op == rs_condition_rs_operator.Between && value is JArray { Count: 2 } bounds
            && Values.Compare(Values.Normalize(bounds[0]), Values.Normalize(bounds[1])) > 0)
        {
            context.Add(CheckContext.Error, ValidationCodes.TypeMismatch, $"{at.Label} uses Between, but the first value is larger than the second.", at);
        }

        if (op is rs_condition_rs_operator.Contains or rs_condition_rs_operator.Startswith && Values.IsEmpty(Values.Normalize(value)))
        {
            context.Add(CheckContext.Error, ValidationCodes.TypeMismatch, $"{at.Label} uses {ChoiceNames.Operator(op)} with nothing to look for.", at);
        }

        if (column?.AttributeType == AttributeTypeCode.Boolean && op is rs_condition_rs_operator.Equals or rs_condition_rs_operator.Notequals or rs_condition_rs_operator.Inlist)
        {
            List<JToken> values = value is JArray list ? [.. list] : [value];
            if (values.Exists(v => !IsYesNo(v)))
            {
                context.Add(CheckContext.Error, ValidationCodes.TypeMismatch, $"{at.Label} compares a Yes/No column with a value that is not yes or no (use true or false).", at);
            }
        }
    }

    private static bool IsYesNo(JToken value) => value.Type switch
    {
        JTokenType.Boolean => true,
        JTokenType.Integer => value.Value<long>() is 0 or 1,
        JTokenType.String => value.Value<string>() is { } text && (bool.TryParse(text, out _) || text is "0" or "1"),
        _ => false,
    };

    private static void CheckFixedShape(CheckContext context, rs_condition_rs_operator op, JToken value, Location at)
    {
        if (op == rs_condition_rs_operator.Inlist && value is not JArray { Count: > 0 })
        {
            context.Add(CheckContext.Error, ValidationCodes.TypeMismatch, $"{at.Label} uses In list, but no list of values is given.", at);
        }
        else if (op == rs_condition_rs_operator.Between && value is not JArray { Count: 2 })
        {
            context.Add(CheckContext.Error, ValidationCodes.TypeMismatch, $"{at.Label} uses Between, which needs a minimum and a maximum.", at);
        }
    }
}
