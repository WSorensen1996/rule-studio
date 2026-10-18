// Rules with the trigger, priority and stop flag of their live and draft versions. The shared
// loadRules() lacks these columns, so this page reads its own.
import { XrmQuery } from "@delegateas/xrmquery";

import type { rs_ruleversion_rs_status as VersionStatus } from "../../../typings/XRM/OptionSets";

const MAX_RULES = 5000;

export interface ChainVersion {
  id: string;
  versionNumber: number | null;
  status: VersionStatus | null;
  targetTable: string | null;
  event: number | null;
  stage: number | null;
  priority: number | null;
  filteredColumns: string;
  stopProcessing: boolean;
}

export interface ChainRule {
  id: string;
  name: string;
  ruleKey: string;
  enabled: boolean;
  live: ChainVersion | null;
  draft: ChainVersion | null;
}

type VersionRow = XDT.rs_RuleVersion_Result | null | undefined;

function toVersion(row: VersionRow): ChainVersion | null {
  if (!row?.rs_ruleversionid) return null;
  return {
    id: row.rs_ruleversionid,
    versionNumber: row.rs_versionnumber,
    status: row.rs_status,
    targetTable: row.rs_targettable,
    event: row.rs_event,
    stage: row.rs_stage,
    priority: row.rs_priority,
    filteredColumns: row.rs_filteredcolumns ?? "",
    stopProcessing: row.rs_stopprocessing === true,
  };
}

/** Every rule with its live and draft version. */
export async function loadChainRules(): Promise<ChainRule[]> {
  const versionColumns = (v: XDT.rs_RuleVersion_Select) => [
    v.rs_ruleversionid, v.rs_versionnumber, v.rs_status, v.rs_targettable, v.rs_event, v.rs_stage,
    v.rs_priority, v.rs_filteredcolumns, v.rs_stopprocessing,
  ] as const;
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_rules)
    .select((x) => [x.rs_ruleid, x.rs_name, x.rs_rulekey, x.rs_enabled])
    .expand((x) => x.rs_PublishedVersionId, versionColumns)
    .expand((x) => x.rs_DraftVersionId, versionColumns)
    .top(MAX_RULES)
    .promise();
  return rows.map((row) => ({
    id: row.rs_ruleid,
    name: row.rs_name ?? "Unnamed rule",
    ruleKey: row.rs_rulekey ?? "",
    enabled: row.rs_enabled !== false,
    live: toVersion(row.rs_PublishedVersionId),
    draft: toVersion(row.rs_DraftVersionId),
  }));
}
