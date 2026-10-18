namespace DataverseLogic.RuleStudio.Engine;

/// <summary>Error codes the engine writes to rs_trace error.code. The list is open.</summary>
public static class EngineCodes
{
    public const string RuleFailed = "RULE_FAILED";
    public const string FieldNotFound = "FIELD_NOT_FOUND";
    public const string ChildPathWithoutAggregate = "CHILD_PATH_WITHOUT_AGGREGATE";
    public const string HopLimitExceeded = "HOP_LIMIT_EXCEEDED";
    public const string RowLimitExceeded = "ROW_LIMIT_EXCEEDED";
    public const string OperatorNotSupported = "OPERATOR_NOT_SUPPORTED";
    public const string ExpressionFailed = "EXPRESSION_FAILED";
    public const string LookupNotFound = "LOOKUP_INVALID";
    public const string LookupNoMatch = "LOOKUP_NO_MATCH";
    public const string LookupMultipleMatches = "LOOKUP_MULTIPLE_MATCHES";
    public const string FunctionNotRegistered = "FUNCTION_NOT_REGISTERED";
    public const string FunctionFailed = "FUNCTION_FAILED";
    public const string FunctionTimeout = "FUNCTION_TIMEOUT";
    public const string BlockSaveAfterSave = "BLOCK_SAVE_AFTER_SAVE";
    public const string BlockedByRule = "BLOCKED";
}
