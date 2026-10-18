// Client for rs_ExportRules and rs_ImportRules (docs/custom-apis.md, json-formats.md §6).
import { ApiError, callAction } from "./releaseApi";

import type { ValidationIssue } from "./formats";

export type ImportStatus = "create" | "update" | "unchanged" | "keepTarget" | "conflict" | "targetOnly";

export interface ImportRuleResult {
  ruleKey: string;
  name?: string;
  status: ImportStatus;
  incomingHash?: string;
  currentHash?: string;
  baseHash?: string;
  message?: string;
  /** conflict only: the incoming portable rule. */
  incoming?: unknown;
  /** conflict and targetOnly: this environment's portable rule. */
  current?: unknown;
}

export interface ImportReport {
  version: number;
  mode: "check" | "apply";
  /** clean: check passed; blocked: conflicts or errors, nothing changed; applied. */
  outcome: "clean" | "blocked" | "applied";
  source?: string;
  releaseId?: string;
  rules: ImportRuleResult[];
  lookups: { name: string; status: "create" | "update" | "unchanged" }[];
  issues: ValidationIssue[];
}

/** The published rules (all, or these keys) as a portable bundle; returns the bundle's JSON text. */
export async function exportRules(ruleKeys?: string[]): Promise<string> {
  const output = await callAction("rs_ExportRules", ruleKeys?.length ? { RuleKeys: JSON.stringify(ruleKeys) } : {});
  if (typeof output.Bundle !== "string") throw new ApiError("The response has no Bundle.", 200);
  return output.Bundle;
}

/** Checks (no changes) or applies a bundle with the three-way merge. */
export async function importRules(bundle: string, mode: "check" | "apply"): Promise<ImportReport> {
  const output = await callAction("rs_ImportRules", { Bundle: bundle, Mode: mode });
  if (typeof output.Report !== "string") throw new ApiError("The response has no Report.", 200);
  try {
    return JSON.parse(output.Report) as ImportReport;
  } catch {
    throw new ApiError("The import report could not be read.", 200);
  }
}
