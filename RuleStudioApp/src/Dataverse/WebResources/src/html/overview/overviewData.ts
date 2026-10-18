// Everything the overview dashboard shows, loaded with a few small queries in parallel.
import { Filter, XrmQuery } from "@delegateas/xrmquery";

import { rs_executionlog_rs_outcome as Outcome, rs_ruleversion_rs_status as VersionStatus } from "../../../typings/XRM/OptionSets";
import { loadRules } from "../../shared/data/rules";

import type { RuleSummary } from "../../shared/data/rules";

const MAX_COUNT = 5000;
const RECENT = 5;
const LOG_WINDOW_DAYS = 7;

export interface RecentDraft {
  ruleId: string;
  ruleName: string;
  ruleKey: string;
  versionNumber: number | null;
  modified: string;
}

export interface Failure {
  ruleName: string;
  message: string;
  when: string;
}

export interface ReleaseItem {
  name: string;
  status: string;
  statusValue: number | null;
  when: string;
}

export interface Overview {
  rules: RuleSummary[];
  drafts: RecentDraft[];
  runs: number;
  failedRuns: number;
  failures: Failure[];
  releases: ReleaseItem[];
}

async function recentDrafts(rules: RuleSummary[]): Promise<RecentDraft[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_ruleversions)
    .select((x) => [x.rs_ruleid_guid, x.rs_versionnumber, x.modifiedon])
    .filter((x) => Filter.equals(x.rs_status, VersionStatus.Draft))
    .orderDesc((x) => x.modifiedon)
    .top(RECENT)
    .includeFormattedValues()
    .promise();
  return rows.flatMap((row) => {
    const rule = rules.find((r) => r.id === row.rs_ruleid_guid);
    if (!rule) return [];
    return [{ ruleId: rule.id, ruleName: rule.name, ruleKey: rule.ruleKey, versionNumber: row.rs_versionnumber, modified: row.modifiedon_formatted ?? "" }];
  });
}

/** Non-test executions of the last week: only the outcome, to count runs and failures. */
async function recentRuns(): Promise<{ runs: number; failed: number }> {
  const since = new Date(Date.now() - LOG_WINDOW_DAYS * 24 * 60 * 60 * 1000);
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_executionlogs)
    .select((x) => [x.rs_outcome])
    .filter((x) => Filter.and(Filter.greaterThan(x.createdon, since), Filter.notEquals(x.rs_istest, true)))
    .top(MAX_COUNT)
    .promise();
  return { runs: rows.length, failed: rows.filter((row) => row.rs_outcome === Outcome.Failed).length };
}

async function recentFailures(): Promise<Failure[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_executionlogs)
    .select((x) => [x.rs_ruleid_guid, x.rs_errormessage, x.createdon])
    .filter((x) => Filter.and(Filter.equals(x.rs_outcome, Outcome.Failed), Filter.notEquals(x.rs_istest, true)))
    .orderDesc((x) => x.createdon)
    .top(RECENT)
    .includeFormattedValues()
    .promise();
  return rows.map((row) => ({
    ruleName: row.rs_ruleid_formatted ?? "Unknown rule",
    message: row.rs_errormessage ?? "No error message recorded.",
    when: row.createdon_formatted ?? "",
  }));
}

async function recentReleases(): Promise<ReleaseItem[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_releases)
    .select((x) => [x.rs_name, x.rs_status, x.createdon])
    .orderDesc((x) => x.createdon)
    .top(RECENT)
    .includeFormattedValues()
    .promise();
  return rows.map((row) => ({
    name: row.rs_name ?? "Unnamed release",
    status: row.rs_status_formatted ?? "",
    statusValue: row.rs_status,
    when: row.createdon_formatted ?? "",
  }));
}

export async function loadOverview(): Promise<Overview> {
  const rules = await loadRules();
  const [drafts, runs, failures, releases] = await Promise.all([
    recentDrafts(rules), recentRuns(), recentFailures(), recentReleases(),
  ]);
  return { rules, drafts, runs: runs.runs, failedRuns: runs.failed, failures, releases };
}
