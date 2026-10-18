// Execution logs (rs_executionlog) as the execution logs page and the log detail pane need them.
// One row = one rule version run against one record; rows of one save share rs_correlationid.
// The engine writes them (docs/engine.md); test runs are not logged (rs_SimulateRule saves nothing).
import { XrmQuery, Filter } from "@delegateas/xrmquery";

import { rs_executionlog_rs_outcome } from "../../../typings/XRM/OptionSets";
import { pill } from "../dom";

const MAX_LOGS = 500;
const MAX_CORRELATED = 100;

export interface LogSummary {
  id: string;
  ruleId: string | null;
  ruleName: string;
  ruleVersionId: string | null;
  versionName: string;
  table: string;
  recordId: string;
  trigger: string;
  outcome: number | null;
  durationMs: number | null;
  attempts: number | null;
  correlationId: string;
  isTest: boolean;
  adminFlag: boolean;
  createdOn: string;
}

export interface LogDetail extends LogSummary {
  logLevel: string;
  errorMessage: string;
  trace: string | null;
}

export type LogMode = "all" | "production" | "test";

export interface LogFilter {
  ruleId: string;
  table: string;
  /** rs_outcome value as text, or "" for any; "errors" means Failed or Blocked. */
  outcome: string;
  /** yyyy-mm-dd in local time, or "". */
  from: string;
  to: string;
  mode: LogMode;
  flaggedOnly: boolean;
  correlationId: string;
}

export interface RuleOption {
  id: string;
  name: string;
}

type LogRow = Pick<XDT.rs_ExecutionLog_Result, "rs_executionlogid" | "rs_ruleid_guid" | "rs_ruleversionid_guid" | "rs_targettable"
  | "rs_recordid" | "rs_outcome" | "rs_durationms" | "rs_attempts" | "rs_correlationid" | "rs_istest" | "rs_adminflag">
  & XDT.rs_ExecutionLog_FormattedResult;

function toSummary(row: LogRow): LogSummary {
  const trigger = [row.rs_targettable, row.rs_event_formatted, row.rs_stage_formatted].filter(Boolean).join(" · ");
  return {
    id: row.rs_executionlogid,
    ruleId: row.rs_ruleid_guid,
    ruleName: row.rs_ruleid_formatted ?? "Unknown rule",
    ruleVersionId: row.rs_ruleversionid_guid,
    versionName: row.rs_ruleversionid_formatted ?? "",
    table: row.rs_targettable ?? "",
    recordId: row.rs_recordid ?? "",
    trigger: trigger || "—",
    outcome: row.rs_outcome,
    durationMs: row.rs_durationms,
    attempts: row.rs_attempts,
    correlationId: row.rs_correlationid ?? "",
    isTest: row.rs_istest === true,
    adminFlag: row.rs_adminflag === true,
    createdOn: row.createdon_formatted ?? "—",
  };
}

/** Start of the given local day, or of the day after it (for an inclusive "to" date). */
function localDay(text: string, nextDay: boolean): Date | null {
  const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(text);
  if (!match) return null;
  return new Date(Number(match[1]), Number(match[2]) - 1, Number(match[3]) + (nextDay ? 1 : 0));
}

function outcomeFilter(x: XDT.rs_ExecutionLog_Filter, outcome: string): WebFilter | null {
  if (outcome === "errors") return Filter.$in(x.rs_outcome, [rs_executionlog_rs_outcome.Failed, rs_executionlog_rs_outcome.Blocked]);
  if (!outcome) return null;
  return Filter.equals(x.rs_outcome, Number(outcome) as rs_executionlog_rs_outcome);
}

function modeFilter(x: XDT.rs_ExecutionLog_Filter, mode: LogMode): WebFilter | null {
  if (mode === "production") return Filter.notEquals(x.rs_istest, true);
  if (mode === "test") return Filter.equals(x.rs_istest, true);
  return null;
}

function buildFilter(x: XDT.rs_ExecutionLog_Filter, filter: LogFilter): WebFilter[] {
  const from = localDay(filter.from, false);
  const to = localDay(filter.to, true);
  const parts: (WebFilter | null)[] = [
    filter.ruleId ? Filter.equals(x.rs_ruleid_guid, Filter.makeGuid(filter.ruleId)) : null,
    filter.table ? Filter.equals(x.rs_targettable, filter.table.trim().toLowerCase()) : null,
    filter.correlationId ? Filter.equals(x.rs_correlationid, filter.correlationId.trim()) : null,
    outcomeFilter(x, filter.outcome),
    modeFilter(x, filter.mode),
    filter.flaggedOnly ? Filter.equals(x.rs_adminflag, true) : null,
    from ? Filter.greaterThanOrEqual(x.createdon, from) : null,
    to ? Filter.lessThan(x.createdon, to) : null,
  ];
  return parts.filter((part): part is WebFilter => part !== null);
}

function hasCriteria(filter: LogFilter): boolean {
  const texts = [filter.ruleId, filter.table, filter.correlationId, filter.outcome, filter.from, filter.to];
  return texts.some((text) => text.trim() !== "") || filter.mode !== "all" || filter.flaggedOnly;
}

/** The newest logs matching the filter (at most 500). */
export async function loadLogs(filter: LogFilter): Promise<LogSummary[]> {
  const all = XrmQuery.retrieveMultiple((x) => x.rs_executionlogs);
  const query = hasCriteria(filter) ? all.filter((x) => Filter.ands(buildFilter(x, filter))) : all;
  const rows = await query
    .select((x) => [x.rs_executionlogid, x.rs_ruleid_guid, x.rs_ruleversionid_guid, x.rs_targettable, x.rs_recordid, x.rs_event,
      x.rs_stage, x.rs_outcome, x.rs_durationms, x.rs_attempts, x.rs_correlationid, x.rs_istest, x.rs_adminflag, x.createdon])
    .orderDesc((x) => x.createdon)
    .top(MAX_LOGS)
    .includeFormattedValues()
    .promise();
  return rows.map(toSummary);
}

export async function loadLog(id: string): Promise<LogDetail> {
  const row = await XrmQuery.retrieve((x) => x.rs_executionlogs, id)
    .select((x) => [x.rs_executionlogid, x.rs_ruleid_guid, x.rs_ruleversionid_guid, x.rs_targettable, x.rs_recordid, x.rs_event,
      x.rs_stage, x.rs_outcome, x.rs_durationms, x.rs_attempts, x.rs_correlationid, x.rs_istest, x.rs_adminflag, x.createdon,
      x.rs_loglevel, x.rs_errormessage, x.rs_trace])
    .includeFormattedValues()
    .promise();
  return { ...toSummary(row), logLevel: row.rs_loglevel_formatted ?? "Not recorded", errorMessage: row.rs_errormessage ?? "", trace: row.rs_trace };
}

/** The other rule runs of the same save, oldest first. */
export async function loadCorrelated(correlationId: string, excludeId: string): Promise<LogSummary[]> {
  if (!correlationId) return [];
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_executionlogs)
    .select((x) => [x.rs_executionlogid, x.rs_ruleid_guid, x.rs_ruleversionid_guid, x.rs_targettable, x.rs_recordid, x.rs_event,
      x.rs_stage, x.rs_outcome, x.rs_durationms, x.rs_attempts, x.rs_correlationid, x.rs_istest, x.rs_adminflag, x.createdon])
    .filter((x) => Filter.equals(x.rs_correlationid, correlationId))
    .orderAsc((x) => x.createdon)
    .top(MAX_CORRELATED)
    .includeFormattedValues()
    .promise();
  return rows.map(toSummary).filter((log) => log.id !== excludeId);
}

/** Active rules for the rule filter, by name. */
export async function loadRuleOptions(): Promise<RuleOption[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_rules)
    .select((x) => [x.rs_ruleid, x.rs_name, x.rs_rulekey])
    .orderAsc((x) => x.rs_name)
    .promise();
  return rows.map((row) => ({ id: row.rs_ruleid, name: row.rs_name ?? row.rs_rulekey ?? "Unnamed rule" }));
}

// ── Labels ───────────────────────────────────────────────────────────────

const OUTCOMES: Record<number, { text: string; kind: "live" | "bad" | "muted" }> = {
  [rs_executionlog_rs_outcome.Matched]: { text: "Matched", kind: "live" },
  [rs_executionlog_rs_outcome.Notmatched]: { text: "Not matched", kind: "muted" },
  [rs_executionlog_rs_outcome.Skipped]: { text: "Skipped", kind: "muted" },
  [rs_executionlog_rs_outcome.Failed]: { text: "Failed", kind: "bad" },
  [rs_executionlog_rs_outcome.Blocked]: { text: "Blocked", kind: "bad" },
};

export const OUTCOME_OPTIONS = Object.entries(OUTCOMES).map(([value, { text }]) => ({ value, label: text }));

export function logOutcomePill(outcome: number | null): HTMLSpanElement {
  const known = outcome === null ? undefined : OUTCOMES[outcome];
  return known ? pill(known.text, known.kind) : pill("Unknown", "muted");
}

export function modePill(log: LogSummary): HTMLSpanElement {
  return log.isTest ? pill("Test", "draft") : pill("Production", "info");
}

export function flagPill(adminFlag: boolean): HTMLSpanElement {
  return adminFlag ? pill("Flagged for review", "warning") : pill("Not flagged", "muted");
}

export function durationText(ms: number | null): string {
  return ms === null ? "—" : `${ms} ms`;
}
