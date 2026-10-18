// JSON formats stored in Rule Studio memo columns. Specification: docs/json-formats.md.
// Every document carries `version: 1`; readers reject versions they do not know.

export const FORMAT_VERSION = 1;

// ── Shared building blocks ───────────────────────────────────────────────

/** One step from a table to a related table. */
export interface FieldPathHop {
  /** parent: lookup on the current table (N:1). child: lookup on the other table pointing back (1:N). */
  kind: "parent" | "child";
  /** parent: lookup column on the current table. child: lookup column on the child table. */
  attribute: string;
  /** Logical name of the table reached. */
  table: string;
}

/** Route from the triggering record to a column. Also the format of rs_fieldpath and rs_valuepath. */
export interface FieldPath {
  table: string;
  hops: FieldPathHop[];
  column: string;
}

export interface RecordReference {
  table: string;
  id: string;
  /** Display only. */
  name?: string;
}

/** Fixed value: text, number, boolean, null or a record reference. Dates are ISO strings, choices their integer value. */
export type FixedValue = string | number | boolean | null | RecordReference;

export type ValueSource =
  | { type: "fixed"; value: FixedValue }
  | { type: "fieldPath"; path: FieldPath }
  | { type: "expression"; expression: string }
  | { type: "lookup"; lookupDefinitionId: string }
  | { type: "function"; customFunctionId: string };

/** Value sources allowed inside match criteria (no nested lookups or functions). */
export type CriterionValueSource = Extract<ValueSource, { type: "fixed" | "fieldPath" | "expression" }>;

// ── 1. rs_lookupdefinition.rs_matchcriteria ─────────────────────────────────

export type MatchCriterion =
  | { operator: "equals"; column: string; value: CriterionValueSource; emptyMatchesAll?: boolean }
  | { operator: "in"; column: string; values: CriterionValueSource[]; emptyMatchesAll?: boolean }
  | { operator: "range"; minColumn: string; maxColumn: string; value: CriterionValueSource; emptyMatchesAll?: boolean }
  | { operator: "isEmpty"; column: string };

export interface MatchCriteria {
  version: typeof FORMAT_VERSION;
  /** Table of the record the lookup is evaluated for; every field path in the criteria starts here. */
  recordTable: string;
  criteria: MatchCriterion[];
}

// ── 2. rs_customfunction.rs_inputschema ─────────────────────────────────────

/** Same values as the rs_customfunction.rs_outputtype choice. */
export type ParameterType = "text" | "number" | "decimal" | "money" | "boolean" | "date" | "lookup";

export interface FunctionParameter {
  /** Custom API request parameter unique name. */
  name: string;
  displayName?: string;
  type: ParameterType;
  /** Expected table, only for type "lookup". */
  table?: string;
  required: boolean;
  description?: string;
}

export interface InputSchema {
  version: typeof FORMAT_VERSION;
  parameters: FunctionParameter[];
}

// ── 3. rs_release.rs_validationresult ───────────────────────────────────────

export type ValidationOutcome = "passed" | "passedWithWarnings" | "failed";
export type CostLevel = "low" | "medium" | "high";
export type IssueLocationKind = "trigger" | "conditionGroup" | "condition" | "action" | "inputMapping" | "release";

/** Known codes; the list is open, so unknown codes must still render. */
export type KnownValidationCode =
  | "TABLE_NOT_FOUND" | "FIELD_NOT_FOUND" | "TYPE_MISMATCH" | "OPERATOR_NOT_ALLOWED"
  | "CHILD_PATH_WITHOUT_AGGREGATE" | "HOP_LIMIT_EXCEEDED" | "FUNCTION_NOT_REGISTERED"
  | "REQUIRED_INPUT_MISSING" | "INPUT_MAPPING_INVALID" | "LOOKUP_INVALID" | "EXPRESSION_INVALID"
  | "BLOCK_SAVE_AFTER_SAVE" | "NO_DRAFT" | "FIELD_SET_CONFLICT" | "RETRIGGER_CHAIN" | "SIDE_EFFECT_FUNCTION"
  | "ENGINE_NOT_REGISTERED" | "STEP_NOT_SUPPORTED";

export interface ValidatedRule {
  ruleId: string;
  ruleKey: string;
  ruleVersionId: string;
  versionNumber: number;
  costLevel?: CostLevel;
}

export interface ValidationIssue {
  severity: "error" | "warning";
  code: KnownValidationCode | (string & Record<never, never>);
  message: string;
  ruleKey?: string;
  ruleVersionId?: string;
  location?: { kind: IssueLocationKind; id: string };
}

export interface ValidationResult {
  version: typeof FORMAT_VERSION;
  checkedOn: string;
  outcome: ValidationOutcome;
  rules: ValidatedRule[];
  issues: ValidationIssue[];
}

// ── 4. rs_executionlog.rs_trace ─────────────────────────────────────────────

export type TraceOutcome = "matched" | "notMatched" | "skipped" | "failed" | "blocked";
export type SkipReason = "ruleDisabled" | "columnsNotChanged" | "stoppedByEarlierRule";
export type TraceOperator =
  | "equals" | "notEquals" | "greaterThan" | "greaterOrEqual" | "lessThan" | "lessOrEqual"
  | "between" | "inList" | "contains" | "startsWith" | "isEmpty" | "isNotEmpty" | "changed";
export type TraceAggregate = "none" | "any" | "all" | "noneOf" | "count" | "sum" | "min" | "max" | "average";

export interface FunctionTrace {
  customFunctionId: string;
  customApiName: string;
  inputs: Record<string, FixedValue>;
  mocked: boolean;
  result: { success: boolean; value?: FixedValue; message?: string };
  durationMs: number;
  /** The call took longer than the function's timeout; its result was discarded. */
  timedOut?: boolean;
}

export interface LookupTrace {
  lookupDefinitionId: string;
  /** Absent when nothing matched. */
  matchedRowId?: string;
  matchCount: number;
  wildcards: number;
  value?: FixedValue;
  /** Present when a no-match or multi-match policy decided the result. */
  policy?: "usedDefault" | "skipped" | "blocked" | "error";
}

export interface ConditionTrace {
  type: "condition";
  id: string;
  path?: FieldPath;
  aggregate?: TraceAggregate;
  operator: TraceOperator;
  expected?: FixedValue;
  actual?: FixedValue;
  /** Any, All and None-of: the child records checked; actual is how many matched. */
  total?: number;
  /** null when short-circuited (notEvaluated). */
  result: boolean | null;
  notEvaluated?: boolean;
  lookup?: LookupTrace;
  function?: FunctionTrace;
}

export interface ConditionGroupTrace {
  type: "group";
  id: string;
  operator: "and" | "or";
  result: boolean | null;
  notEvaluated?: boolean;
  children: (ConditionGroupTrace | ConditionTrace)[];
}

export interface ActionTrace {
  id: string;
  type: "setField" | "blockSave" | "callFunction" | "logMessage";
  status: "done" | "skipped" | "failed";
  field?: string;
  before?: FixedValue;
  after?: FixedValue;
  message?: string;
  messageLevel?: "info" | "warning" | "error";
  lookup?: LookupTrace;
  function?: FunctionTrace;
}

/** The rule version a trace belongs to (added 2026-10-16 with rs_SimulateRule, pending review). */
export interface TraceRule {
  ruleId: string;
  ruleKey: string;
  /** Display only; captured at run time like record.name. */
  name?: string;
  ruleVersionId?: string;
  versionNumber?: number;
  priority: number;
  /** The identity the rule ran with (added 2026-10-17). */
  runAs?: "callingUser" | "system";
  /** Test runs only: the draft was evaluated instead of the published version. */
  draft?: boolean;
}

export interface ExecutionTrace {
  version: typeof FORMAT_VERSION;
  level: "summary" | "fullTrace";
  sequence: number;
  /** Pending review. */
  rule?: TraceRule;
  record: RecordReference;
  startedOn: string;
  durationMs: number;
  outcome: TraceOutcome;
  skipReason?: SkipReason;
  test?: { mockedFunctions: string[] };
  conditions?: ConditionGroupTrace;
  branch?: "then" | "else" | "none";
  actions?: ActionTrace[];
  error?: { code: string; message: string; attempt: number };
  truncated?: boolean;
}

// ── Reading ──────────────────────────────────────────────────────────────

export type ParsedDocument<T> =
  | { ok: true; value: T }
  | { ok: false; reason: "empty" | "invalidJson" | "unsupportedVersion" };

/**
 * Parses a memo column value. Checks the JSON syntax and the version only; the shape is trusted
 * because only Rule Studio writes these columns.
 */
export function parseDocument<T extends { version: number }>(text: string | null | undefined): ParsedDocument<T> {
  if (!text?.trim()) return { ok: false, reason: "empty" };
  let parsed: unknown;
  try {
    parsed = JSON.parse(text);
  } catch {
    return { ok: false, reason: "invalidJson" };
  }
  if (typeof parsed !== "object" || parsed === null || (parsed as { version?: unknown }).version !== FORMAT_VERSION) {
    return { ok: false, reason: "unsupportedVersion" };
  }
  return { ok: true, value: parsed as T };
}
