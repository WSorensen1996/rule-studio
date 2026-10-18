// Rules as the list pages need them: the rule plus a summary of its live and
// draft versions. The trigger lives on the version, so each version carries its own.
import { Filter, XrmQuery } from "@delegateas/xrmquery";

import { rs_release_rs_status } from "../../../typings/XRM/OptionSets";

const MAX_RULES = 5000;

export interface VersionSummary {
  id: string;
  versionNumber: number | null;
  priority: number | null;
  targetTable: string | null;
  event: number | null;
  stage: number | null;
}

export interface RuleSummary {
  id: string;
  name: string;
  ruleKey: string;
  enabled: boolean;
  live: VersionSummary | null;
  draft: VersionSummary | null;
  /** Neither live nor draft: the highest-numbered version, and whether its release was rolled back. */
  latest: VersionSummary | null;
  latestRolledBack: boolean;
}

type VersionRow = Pick<XDT.rs_RuleVersion_Result, "rs_ruleversionid" | "rs_versionnumber" | "rs_priority" | "rs_targettable" | "rs_event" | "rs_stage"> | null | undefined;

function toVersion(row: VersionRow): VersionSummary | null {
  if (!row?.rs_ruleversionid) return null;
  return {
    id: row.rs_ruleversionid,
    versionNumber: row.rs_versionnumber,
    priority: row.rs_priority,
    targetTable: row.rs_targettable,
    event: row.rs_event,
    stage: row.rs_stage,
  };
}

/** All rules, sorted by name, each with its live and draft version summary. */
export async function loadRules(): Promise<RuleSummary[]> {
  const versionColumns = (v: XDT.rs_RuleVersion_Select) =>
    [v.rs_ruleversionid, v.rs_versionnumber, v.rs_priority, v.rs_targettable, v.rs_event, v.rs_stage] as const;
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_rules)
    .select((x) => [x.rs_ruleid, x.rs_name, x.rs_rulekey, x.rs_enabled])
    .expand((x) => x.rs_PublishedVersionId, versionColumns)
    .expand((x) => x.rs_DraftVersionId, versionColumns)
    .orderAsc((x) => x.rs_name)
    .top(MAX_RULES)
    .promise();

  const rules = rows.map((row) => ({
    id: row.rs_ruleid,
    name: row.rs_name ?? "Unnamed rule",
    ruleKey: row.rs_rulekey ?? "",
    enabled: row.rs_enabled !== false,
    live: toVersion(row.rs_PublishedVersionId),
    draft: toVersion(row.rs_DraftVersionId),
    latest: null as VersionSummary | null,
    latestRolledBack: false,
  }));
  await addLatest(rules.filter((rule) => !rule.live && !rule.draft));
  return rules;
}

/** Fills `latest` for rules without a live or draft version, e.g. after rolling back their only release. */
async function addLatest(rules: RuleSummary[]): Promise<void> {
  if (rules.length === 0) return;
  const byId = new Map(rules.map((rule) => [rule.id, rule]));
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_ruleversions)
    .select((x) => [x.rs_ruleversionid, x.rs_ruleid_guid, x.rs_versionnumber, x.rs_priority, x.rs_targettable, x.rs_event, x.rs_stage])
    .expand((x) => x.rs_ReleaseId, (r) => [r.rs_status])
    .filter((x) => [...byId.keys()].map((id) => Filter.equals(x.rs_ruleid_guid, Filter.makeGuid(id))).reduce((a, b) => Filter.or(a, b)))
    .orderDesc((x) => x.rs_versionnumber)
    .top(MAX_RULES)
    .promise();
  for (const row of rows) {
    const rule = row.rs_ruleid_guid ? byId.get(row.rs_ruleid_guid) : undefined;
    if (!rule || rule.latest) continue;
    rule.latest = toVersion(row);
    rule.latestRolledBack = row.rs_ReleaseId?.rs_status === rs_release_rs_status.Rolledback;
  }
}
