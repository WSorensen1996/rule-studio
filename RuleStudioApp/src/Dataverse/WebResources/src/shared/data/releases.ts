// Releases (rs_release) as the releases page and the release detail pane need them, plus the
// labels for release status and validation outcome. Publishing writes these rows: docs/custom-apis.md.
import { XrmQuery, Filter } from "@delegateas/xrmquery";

import { rs_release_rs_status, rs_ruleversion_rs_costlevel, rs_ruleversion_rs_status } from "../../../typings/XRM/OptionSets";
import { h, pill } from "../dom";
import { parseDocument } from "../formats";
import { loadRules } from "./rules";
import { uniqueBy } from "./unique";


import type { ValidationOutcome, ValidationResult } from "../formats";

const MAX_RELEASES = 500;

export interface ReleaseSummary {
  id: string;
  name: string;
  description: string;
  status: number | null;
  publishedOn: string;
  publishedBy: string;
  versionCount: number;
  validation: string | null;
  rolledBackReleaseId: string | null;
  rolledBackReleaseName: string;
  /** Rules whose published version is no longer this release's version; empty when rollback can run. */
  changedRules: string[];
  /** For a rollback release: the number of rules it restored (the rolled-back release's versions). */
  restoredCount: number;
}

export interface ReleasedVersion {
  id: string;
  ruleId: string | null;
  ruleName: string;
  ruleKey: string;
  versionNumber: number | null;
  costLevel: number | null;
  status: number | null;
}

type ReleaseRow = Pick<XDT.rs_Release_Result, "rs_releaseid" | "rs_name" | "rs_description" | "rs_status"
  | "rs_validationresult" | "rs_rolledbackreleaseid_guid"> & XDT.rs_Release_FormattedResult;

function toSummary(row: ReleaseRow, versionCount: number): ReleaseSummary {
  return {
    id: row.rs_releaseid,
    name: row.rs_name ?? "Unnamed release",
    description: row.rs_description ?? "",
    status: row.rs_status,
    publishedOn: row.rs_publishedon_formatted ?? "—",
    publishedBy: row.rs_publishedby_formatted ?? "—",
    versionCount,
    validation: row.rs_validationresult,
    rolledBackReleaseId: row.rs_rolledbackreleaseid_guid,
    rolledBackReleaseName: row.rs_rolledbackreleaseid_formatted ?? "an earlier release",
    changedRules: [],
    restoredCount: 0,
  };
}

/**
 * All releases, newest first, each with the number of rule versions it published, the rules a later
 * release changed (the server refuses rollback then) and, for rollback releases, the rules restored.
 */
export async function loadReleases(): Promise<ReleaseSummary[]> {
  const [rows, rules] = await Promise.all([
    XrmQuery.retrieveMultiple((x) => x.rs_releases)
      .select((x) => [x.rs_releaseid, x.rs_name, x.rs_description, x.rs_status, x.rs_publishedon, x.rs_publishedby_guid,
        x.rs_validationresult, x.rs_rolledbackreleaseid_guid])
      .expand((x) => x.rs_rs_release_rs_ruleversion_rs_releaseid, (v) => [v.rs_ruleversionid, v.rs_ruleid_guid])
      .orderDesc((x) => x.rs_publishedon)
      .top(MAX_RELEASES)
      .includeFormattedValues()
      .promise(),
    loadRules(),
  ]);
  const ruleById = new Map(rules.map((rule) => [rule.id, rule]));
  const summaries = rows.map((row) => {
    const versions = uniqueBy(row.rs_rs_release_rs_ruleversion_rs_releaseid, (v) => v.rs_ruleversionid);
    const summary = toSummary(row, versions.length);
    summary.changedRules = versions
      .filter((v) => v.rs_ruleid_guid && ruleById.get(v.rs_ruleid_guid)?.live?.id !== v.rs_ruleversionid)
      .map((v) => ruleById.get(v.rs_ruleid_guid ?? "")?.name ?? "a deleted rule");
    return summary;
  });
  const counts = new Map(summaries.map((r) => [r.id, r.versionCount]));
  for (const release of summaries) {
    if (release.rolledBackReleaseId) release.restoredCount = counts.get(release.rolledBackReleaseId) ?? 0;
  }
  return summaries;
}

/** "13 rule versions", or "N rules restored" for a rollback release. */
export function scopeText(release: ReleaseSummary): string {
  if (release.rolledBackReleaseId) return release.restoredCount === 1 ? "1 rule restored" : `${release.restoredCount} rules restored`;
  return release.versionCount === 1 ? "1 rule version" : `${release.versionCount} rule versions`;
}

/** Why the server would refuse to roll the release back, or "" (same wording as rs_RollbackRelease). */
export function rollbackBlocker(release: ReleaseSummary): string {
  return release.changedRules.length ? `A later release changed ${release.changedRules.join(", ")}. Roll back the later release first.` : "";
}

export async function loadRelease(id: string): Promise<ReleaseSummary> {
  const row = await XrmQuery.retrieve((x) => x.rs_releases, id)
    .select((x) => [x.rs_releaseid, x.rs_name, x.rs_description, x.rs_status, x.rs_publishedon, x.rs_publishedby_guid,
      x.rs_validationresult, x.rs_rolledbackreleaseid_guid])
    .includeFormattedValues()
    .promise();
  return toSummary(row, 0);
}

/** The rule versions a release published, sorted by rule name. */
export async function loadReleasedVersions(releaseId: string): Promise<ReleasedVersion[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_ruleversions)
    .select((x) => [x.rs_ruleversionid, x.rs_ruleid_guid, x.rs_versionnumber, x.rs_costlevel, x.rs_status])
    .expand((x) => x.rs_RuleId, (r) => [r.rs_name, r.rs_rulekey])
    .filter((x) => Filter.equals(x.rs_releaseid_guid, Filter.makeGuid(releaseId)))
    .promise();
  const versions = rows.map((row) => ({
    id: row.rs_ruleversionid,
    ruleId: row.rs_ruleid_guid,
    ruleName: row.rs_RuleId?.rs_name ?? "Unnamed rule",
    ruleKey: row.rs_RuleId?.rs_rulekey ?? "",
    versionNumber: row.rs_versionnumber,
    costLevel: row.rs_costlevel,
    status: row.rs_status,
  }));
  return versions.sort((a, b) => a.ruleName.localeCompare(b.ruleName));
}

// ── Labels ───────────────────────────────────────────────────────────────

export function releaseStatusPill(status: number | null): HTMLSpanElement {
  if (status === rs_release_rs_status.Live) return pill("Live", "live");
  if (status === rs_release_rs_status.Rolledback) return pill("Rolled back", "warning");
  if (status === rs_release_rs_status.Failed) return pill("Failed", "bad");
  return pill("Unknown", "muted");
}

const OUTCOMES: Record<ValidationOutcome, { text: string; kind: "live" | "warning" | "bad" }> = {
  passed: { text: "Passed", kind: "live" },
  passedWithWarnings: { text: "Passed with warnings", kind: "warning" },
  failed: { text: "Failed", kind: "bad" },
};

const UNREADABLE: Record<string, string> = {
  empty: "Not recorded",
  invalidJson: "Unreadable",
  unsupportedVersion: "Unsupported format",
};

/** Parses rs_validationresult; `problem` explains why there is no result. */
export function readValidation(text: string | null): { result: ValidationResult | null; problem: string } {
  const parsed = parseDocument<ValidationResult>(text);
  if (parsed.ok) return { result: parsed.value, problem: "" };
  return { result: null, problem: UNREADABLE[parsed.reason] ?? "Unreadable" };
}

export function outcomePill(text: string | null): HTMLSpanElement {
  const { result, problem } = readValidation(text);
  if (!result) return pill(problem, "muted");
  const known = OUTCOMES[result.outcome] as (typeof OUTCOMES)[ValidationOutcome] | undefined;
  return known ? pill(known.text, known.kind) : pill(String(result.outcome), "muted");
}

const COST_LABELS: Record<number, string> = {
  [rs_ruleversion_rs_costlevel.Low]: "Low cost",
  [rs_ruleversion_rs_costlevel.Medium]: "Medium cost",
  [rs_ruleversion_rs_costlevel.High]: "High cost",
};

export function costLabel(value: number | null): string {
  return value === null ? "Cost not rated" : (COST_LABELS[value] ?? "Cost not rated");
}

export function versionStatusPill(status: number | null): HTMLSpanElement {
  if (status === rs_ruleversion_rs_status.Published) return pill("Live", "live");
  if (status === rs_ruleversion_rs_status.Superseded) return pill("Superseded", "muted");
  if (status === rs_ruleversion_rs_status.Draft) return pill("Draft", "draft");
  if (status === rs_ruleversion_rs_status.Discarded) return pill("Discarded", "muted");
  return pill("Unknown", "muted");
}

/**
 * The Rollback… button for a live release, as a list so callers can spread it: other releases
 * cannot be rolled back and get none. Disabled for viewers.
 */
export function rollbackButton(release: ReleaseSummary, open: () => void, author: boolean): HTMLButtonElement[] {
  // A rollback release publishes no versions of its own: undo it by publishing again.
  if (release.status !== rs_release_rs_status.Live || release.rolledBackReleaseId) return [];
  const button = h("button", { className: "btn mini", attrs: { type: "button" } }, "Rollback…");
  const blocker = rollbackBlocker(release);
  button.disabled = !author || blocker !== "";
  button.title = !author ? "Requires the Rule Studio Author role." : blocker;
  button.addEventListener("click", open);
  return [button];
}
