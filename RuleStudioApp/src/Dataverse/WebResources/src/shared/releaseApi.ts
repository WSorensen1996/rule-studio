// Client for the release Custom APIs (docs/custom-apis.md). All run as the calling user, so a
// Rule Viewer gets a privilege error from Dataverse rather than a silent no-op.
import { parseDocument } from "./formats";
import { webApiUrl } from "./xrm";

import type { ExecutionTrace, LookupTrace, ValidationResult } from "./formats";

const EMPTY_GUID = "00000000-0000-0000-0000-000000000000";

export class ApiError extends Error {
  constructor(message: string, readonly status: number) {
    super(message);
    this.name = "ApiError";
  }
}

export interface PublishRequest {
  ruleIds: string[];
  name: string;
  description?: string;
}

export interface PublishResponse {
  validation: ValidationResult;
  /** Undefined when validation failed and nothing was published. */
  releaseId: string | undefined;
}

/** POSTs an unbound Custom API and returns its output parameters; Dataverse errors become ApiError. */
export async function callAction(name: string, body: Record<string, unknown>): Promise<Record<string, unknown>> {
  const response = await fetch(`${webApiUrl()}${name}`, {
    method: "POST",
    headers: { "Content-Type": "application/json", Accept: "application/json", "OData-Version": "4.0", "OData-MaxVersion": "4.0" },
    body: JSON.stringify(body),
  });
  const text = await response.text();
  if (!response.ok) {
    let message = `${name} failed (${response.status}).`;
    try {
      message = (JSON.parse(text) as { error?: { message?: string } }).error?.message ?? message;
    } catch {
      // Not JSON; keep the generic message.
    }
    throw new ApiError(message, response.status);
  }
  return JSON.parse(text) as Record<string, unknown>;
}

function readValidation(output: Record<string, unknown>): ValidationResult {
  const parsed = parseDocument<ValidationResult>(output.ValidationResult as string | undefined);
  if (!parsed.ok) throw new ApiError(`The validation result could not be read (${parsed.reason}).`, 200);
  return parsed.value;
}

/** Runs the publish checks on each rule's draft. Saves nothing. */
export async function validateRelease(ruleIds: string[]): Promise<ValidationResult> {
  return readValidation(await callAction("rs_ValidateRelease", { RuleIds: JSON.stringify(ruleIds) }));
}

/** Validates again and, unless that fails, publishes all drafts as one release in one transaction. */
export async function publishRelease(request: PublishRequest): Promise<PublishResponse> {
  const body: Record<string, unknown> = { RuleIds: JSON.stringify(request.ruleIds), Name: request.name };
  if (request.description) body.Description = request.description;
  const output = await callAction("rs_PublishRelease", body);
  const releaseId = typeof output.ReleaseId === "string" && output.ReleaseId !== EMPTY_GUID ? output.ReleaseId : undefined;
  return { validation: readValidation(output), releaseId };
}

function readGuid(output: Record<string, unknown>, name: string): string {
  const value = output[name];
  if (typeof value !== "string" || value === EMPTY_GUID) throw new ApiError(`The response has no ${name}.`, 200);
  return value;
}

/** Puts every rule of a live release back on its previous live version, as a new release. Returns the new release id. */
export async function rollbackRelease(releaseId: string): Promise<string> {
  return readGuid(await callAction("rs_RollbackRelease", { ReleaseId: releaseId }), "ReleaseId");
}

/**
 * Copies a rule version into a new draft of the same rule. `replaceDraft` must be true when the
 * rule already has a draft; that draft is then kept in history as Discarded. Returns the new draft id.
 */
export async function restoreVersion(ruleVersionId: string, replaceDraft: boolean): Promise<string> {
  return readGuid(await callAction("rs_RestoreVersion", { RuleVersionId: ruleVersionId, ReplaceDraft: replaceDraft }), "RuleVersionId");
}

/** What rs_SimulateRule evaluates: either `ruleIds`, or a whole `table`/`event`/`stage` chain. */
export interface SimulateRequest {
  ruleIds?: string[];
  table?: string;
  /** create, update or delete. */
  event?: string;
  /** beforeSave or afterSave. */
  stage?: string;
  /** Evaluate drafts where they exist (default true); false tests only published versions. */
  useDrafts?: boolean;
  /** A real record; sample values are applied over it. */
  recordId?: string;
  /** Column → value. For Update these are the changed columns. */
  sampleValues?: Record<string, unknown>;
  /** Also mock functions without side effects (default true). Side-effect functions are always mocked. */
  mockFunctions?: boolean;
}

// true or left out: absent optional Booleans reach the plug-in as false.
function flag(on: boolean): true | undefined {
  return on ? true : undefined;
}

function simulateBody(request: SimulateRequest): Record<string, unknown> {
  const body: Record<string, unknown> = {
    RuleIds: request.ruleIds?.length ? JSON.stringify(request.ruleIds) : undefined,
    Table: request.table || undefined,
    Event: request.event || undefined,
    Stage: request.stage || undefined,
    PublishedOnly: flag(request.useDrafts === false),
    RecordId: request.recordId || undefined,
    SampleValues: request.sampleValues && Object.keys(request.sampleValues).length > 0 ? JSON.stringify(request.sampleValues) : undefined,
    CallFunctions: flag(request.mockFunctions === false),
  };
  return Object.fromEntries(Object.entries(body).filter(([, value]) => value !== undefined));
}

/** Runs rules against a real or sample record without saving anything. Returns one trace per rule, in chain order. */
export async function simulate(request: SimulateRequest): Promise<ExecutionTrace[]> {
  const body = simulateBody(request);
  const output = await callAction("rs_SimulateRule", body);
  const text = output.Trace;
  if (typeof text !== "string") throw new ApiError("The response has no Trace.", 200);
  try {
    return JSON.parse(text) as ExecutionTrace[];
  } catch {
    throw new ApiError("The trace could not be read.", 200);
  }
}

/** rs_SimulateRule's lookup test: one lookup definition evaluated against a record. */
export interface LookupTestResult {
  lookup?: LookupTrace;
  error?: { code: string; message: string; attempt: number };
}

/** Evaluates one lookup definition against a real record of `table` and/or sample values. Saves nothing. */
export async function testLookup(lookupDefinitionId: string, table: string, recordId?: string, sampleValues?: Record<string, unknown>): Promise<LookupTestResult> {
  const body = { ...simulateBody({ table, recordId, sampleValues }), LookupDefinitionId: lookupDefinitionId };
  const text = (await callAction("rs_SimulateRule", body)).Trace;
  if (typeof text !== "string") throw new ApiError("The response has no Trace.", 200);
  try {
    return JSON.parse(text) as LookupTestResult;
  } catch {
    throw new ApiError("The lookup result could not be read.", 200);
  }
}
