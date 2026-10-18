namespace SharedDomain.RuleStudio.Formats;

/// <summary>Known validation codes. The list is open: readers must still show unknown codes.</summary>
public static class ValidationCodes
{
    public const string TableNotFound = "TABLE_NOT_FOUND";
    public const string FieldNotFound = "FIELD_NOT_FOUND";
    public const string TypeMismatch = "TYPE_MISMATCH";
    public const string OperatorNotAllowed = "OPERATOR_NOT_ALLOWED";
    public const string ChildPathWithoutAggregate = "CHILD_PATH_WITHOUT_AGGREGATE";
    public const string HopLimitExceeded = "HOP_LIMIT_EXCEEDED";
    public const string FunctionNotRegistered = "FUNCTION_NOT_REGISTERED";
    public const string RequiredInputMissing = "REQUIRED_INPUT_MISSING";
    public const string InputMappingInvalid = "INPUT_MAPPING_INVALID";
    public const string LookupInvalid = "LOOKUP_INVALID";
    public const string ExpressionInvalid = "EXPRESSION_INVALID";
    public const string BlockSaveAfterSave = "BLOCK_SAVE_AFTER_SAVE";
    public const string NoDraft = "NO_DRAFT";
    public const string FieldSetConflict = "FIELD_SET_CONFLICT";
    public const string RetriggerChain = "RETRIGGER_CHAIN";
    public const string SideEffectFunction = "SIDE_EFFECT_FUNCTION";
    public const string EngineNotRegistered = "ENGINE_NOT_REGISTERED";
    public const string StepNotSupported = "STEP_NOT_SUPPORTED";
    public const string ImportConflict = "IMPORT_CONFLICT";
    public const string ImportFunctionMissing = "IMPORT_FUNCTION_MISSING";
    public const string ImportLookupMissing = "IMPORT_LOOKUP_MISSING";
    public const string ImportInvalidRule = "IMPORT_INVALID_RULE";
    public const string ImportDraftDiscarded = "IMPORT_DRAFT_DISCARDED";
}
