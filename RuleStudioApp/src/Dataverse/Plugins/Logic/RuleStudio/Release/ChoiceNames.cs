using RuleStudio.SharedContext;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>Choice values as the camelCase names used in the JSON formats (docs/json-formats.md).</summary>
internal static class ChoiceNames
{
    internal static string Event(rs_event? value) => value switch
    {
        rs_event.Create => "create",
        rs_event.Update => "update",
        rs_event.Delete => "delete",
        _ => string.Empty,
    };

    internal static string Stage(rs_stage? value) => value switch
    {
        rs_stage.Beforesave => "beforeSave",
        rs_stage.Aftersave => "afterSave",
        _ => string.Empty,
    };

    internal static string Logic(rs_conditiongroup_rs_logicaloperator? value) =>
        value == rs_conditiongroup_rs_logicaloperator.OR ? "or" : "and";

    internal static string Aggregate(rs_condition_rs_aggregate? value) => value switch
    {
        rs_condition_rs_aggregate.Any => "any",
        rs_condition_rs_aggregate.All => "all",
        rs_condition_rs_aggregate.Noneof => "noneOf",
        rs_condition_rs_aggregate.Count => "count",
        rs_condition_rs_aggregate.Sum => "sum",
        rs_condition_rs_aggregate.Min => "min",
        rs_condition_rs_aggregate.Max => "max",
        rs_condition_rs_aggregate.Average => "average",
        _ => "none",
    };

    internal static string Operator(rs_condition_rs_operator? value) => value switch
    {
        rs_condition_rs_operator.Equals => "equals",
        rs_condition_rs_operator.Notequals => "notEquals",
        rs_condition_rs_operator.Greaterthan => "greaterThan",
        rs_condition_rs_operator.Greaterorequal => "greaterOrEqual",
        rs_condition_rs_operator.Lessthan => "lessThan",
        rs_condition_rs_operator.Lessorequal => "lessOrEqual",
        rs_condition_rs_operator.Contains => "contains",
        rs_condition_rs_operator.Startswith => "startsWith",
        rs_condition_rs_operator.Isempty => "isEmpty",
        rs_condition_rs_operator.Isnotempty => "isNotEmpty",
        rs_condition_rs_operator.Changed => "changed",
        rs_condition_rs_operator.Inlist => "inList",
        rs_condition_rs_operator.Between => "between",
        _ => string.Empty,
    };

    internal static string ActionType(rs_action_rs_actiontype? value) => value switch
    {
        rs_action_rs_actiontype.Setfield => "setField",
        rs_action_rs_actiontype.Blocksave => "blockSave",
        rs_action_rs_actiontype.Callfunction => "callFunction",
        rs_action_rs_actiontype.Logmessage => "logMessage",
        _ => string.Empty,
    };

    internal static string? MessageLevel(rs_messagelevel? value) => value switch
    {
        rs_messagelevel.Info => "info",
        rs_messagelevel.Warning => "warning",
        rs_messagelevel.Error => "error",
        _ => null,
    };

    internal static string FailurePolicy(rs_ruleversion_rs_failurepolicy? value) =>
        value == rs_ruleversion_rs_failurepolicy.Logandcontinue ? "logAndContinue" : "block";

    internal static string RunAs(rs_ruleversion_rs_runas? value) =>
        value == rs_ruleversion_rs_runas.System ? "system" : "callingUser";

    internal static string? LogLevel(rs_loglevel? value) => value switch
    {
        rs_loglevel.Off => "off",
        rs_loglevel.Errorsonly => "errorsOnly",
        rs_loglevel.Summary => "summary",
        rs_loglevel.Fulltrace => "fullTrace",
        _ => null,
    };

    internal static string NoMatch(rs_lookupdefinition_rs_nomatchpolicy? value) => value switch
    {
        rs_lookupdefinition_rs_nomatchpolicy.Usedefault => "useDefault",
        rs_lookupdefinition_rs_nomatchpolicy.Skip => "skip",
        _ => "block",
    };

    internal static string MultiMatch(rs_lookupdefinition_rs_multimatchpolicy? value) => value switch
    {
        rs_lookupdefinition_rs_multimatchpolicy.Firstbysort => "firstBySort",
        rs_lookupdefinition_rs_multimatchpolicy.Error => "error",
        _ => "mostSpecific",
    };

    internal static string? OutputType(rs_customfunction_rs_outputtype? value) => value switch
    {
        rs_customfunction_rs_outputtype.Text => "text",
        rs_customfunction_rs_outputtype.Number => "number",
        rs_customfunction_rs_outputtype.Money => "money",
        rs_customfunction_rs_outputtype.Boolean => "boolean",
        rs_customfunction_rs_outputtype.Date => "date",
        rs_customfunction_rs_outputtype.Lookup => "lookup",
        _ => null,
    };

    internal static string ValueSourceType(rs_valuesourcetype? value) => value switch
    {
        rs_valuesourcetype.Fieldpath => "fieldPath",
        rs_valuesourcetype.Expression => "expression",
        rs_valuesourcetype.Lookup => "lookup",
        rs_valuesourcetype.Function => "function",
        _ => "fixed",
    };
}
