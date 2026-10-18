// What rs_RollbackRelease will do, worked out from the same rows it reads (docs/custom-apis.md):
// each rule goes back to its latest superseded version published earlier (skipping versions of
// rolled-back releases), or stops running when this release first published it.
import { Filter, XrmQuery } from "@delegateas/xrmquery";

import { rs_release_rs_status, rs_ruleversion_rs_status } from "../../../typings/XRM/OptionSets";

export interface PlanLine {
  ruleName: string;
  ruleKey: string;
  versionNumber: number | null;
  /** A later release changed the rule, so the rollback is blocked. */
  blocked: boolean;
  /** The version that becomes live again; null when the rule stops running. */
  previousNumber: number | null;
}

interface Candidate {
  ruleId: string;
  versionNumber: number | null;
  publishedOn: number;
}

const time = (value: Date | string | null | undefined): number => (value ? new Date(value).getTime() : Number.NaN);

async function loadCandidates(ruleIds: string[]): Promise<Candidate[]> {
  if (ruleIds.length === 0) return [];
  // In() takes PropertyValues as a JSON string array, so ids must be quoted strings, not bare makeGuid values.
  const ids = [...new Set(ruleIds)] as unknown as XQW.Guid[];
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_ruleversions)
    .select((x) => [x.rs_ruleid_guid, x.rs_versionnumber, x.rs_publishedon])
    .expand((x) => x.rs_ReleaseId, (r) => [r.rs_status])
    .filter((x) => Filter.and(Filter.$in(x.rs_ruleid_guid, ids), Filter.equals(x.rs_status, rs_ruleversion_rs_status.Superseded)))
    .execute();
  return rows
    .filter((row) => row.rs_ruleid_guid && row.rs_ReleaseId && row.rs_ReleaseId.rs_status !== rs_release_rs_status.Rolledback)
    .map((row) => ({ ruleId: (row.rs_ruleid_guid ?? "").toLowerCase(), versionNumber: row.rs_versionnumber, publishedOn: time(row.rs_publishedon) }));
}

/** One line per rule version the release published, sorted by rule name. */
export async function loadPlan(releaseId: string): Promise<PlanLine[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_ruleversions)
    .select((x) => [x.rs_ruleversionid, x.rs_ruleid_guid, x.rs_versionnumber, x.rs_publishedon])
    .expand((x) => x.rs_RuleId, (r) => [r.rs_name, r.rs_rulekey, r.rs_publishedversionid_guid])
    .filter((x) => Filter.equals(x.rs_releaseid_guid, Filter.makeGuid(releaseId)))
    .execute();
  const candidates = await loadCandidates(rows.map((row) => row.rs_ruleid_guid ?? "").filter(Boolean));
  const lines = rows.map((row): PlanLine => {
    const ruleId = (row.rs_ruleid_guid ?? "").toLowerCase();
    const published = time(row.rs_publishedon);
    const previous = candidates
      .filter((c) => c.ruleId === ruleId && c.publishedOn < published)
      .sort((a, b) => b.publishedOn - a.publishedOn)[0] as Candidate | undefined;
    return {
      ruleName: row.rs_RuleId?.rs_name ?? "Unnamed rule",
      ruleKey: row.rs_RuleId?.rs_rulekey ?? "",
      versionNumber: row.rs_versionnumber,
      blocked: (row.rs_RuleId?.rs_publishedversionid_guid ?? "").toLowerCase() !== row.rs_ruleversionid.toLowerCase(),
      previousNumber: previous ? previous.versionNumber ?? 0 : null,
    };
  });
  return lines.sort((a, b) => a.ruleName.localeCompare(b.ruleName));
}
