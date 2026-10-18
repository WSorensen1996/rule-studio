import { loadOverview } from "./overviewData";
import { draftLine, emptyLine, failureLine, metric, releaseLine, triggerBar, triggers } from "./overviewView";
import { applyRole } from "../../shared/auth";
import { byId } from "../../shared/dom";
import { tableDisplayNames } from "../../shared/metadata";
import { navigateToPage, openRecord } from "../../shared/nav";
import { describeError, setStatus } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { Overview } from "./overviewData";
import type { PageName, Params } from "../../shared/nav";

applyStandard();

const statusLine = byId("status");

function plural(count: number, one: string, many: string): string {
  return `${count} ${count === 1 ? one : many}`;
}

async function go(page: PageName, params?: Params): Promise<void> {
  try {
    await navigateToPage(page, params);
  } catch (error) {
    setStatus(statusLine, describeError(`open ${page}`, error), "error");
  }
}

async function openEditor(params: Params): Promise<void> {
  try {
    await openRecord("rule-editor", params);
  } catch (error) {
    setStatus(statusLine, describeError("open the rule editor", error), "error");
  }
}

function renderMetrics(data: Overview, tables: string[]): void {
  const rules = data.rules;
  const live = rules.filter((r) => r.live !== null).length;
  const drafts = rules.filter((r) => r.draft !== null).length;
  const disabled = rules.filter((r) => !r.enabled).length;
  byId("metricTotal").replaceChildren(metric("▤", "Total rules", rules.length, `Across ${plural(tables.length, "Dataverse table", "Dataverse tables")}`));
  byId("metricLive").replaceChildren(metric("●", "Live rules", live, "Active published snapshots"));
  byId("metricDrafts").replaceChildren(metric("✎", "Drafts in progress", drafts, "Not affecting live execution"));
  byId("metricUnpublished").replaceChildren(metric("○", "Not published", rules.length - live, `${disabled} inactive`));
}

function renderLists(data: Overview, tableNames: Map<string, string>): void {
  const drafts = data.drafts.map((d) => draftLine(d, (ruleId) => void openEditor({ ruleId })));
  byId("recentDrafts").replaceChildren(...(drafts.length ? drafts : [emptyLine("No drafts in progress.")]));
  const bars = triggers(data.rules, tableNames);
  const max = Math.max(1, ...bars.map((b) => b.count));
  byId("triggerBars").replaceChildren(...(bars.length ? bars.map((b, i) => triggerBar(b, max, i)) : [emptyLine("No rule has a trigger yet.")]));
  byId("runCount").textContent = data.runs ? `${plural(data.runs, "run", "runs")} · ${data.failedRuns} failed (7 days)` : "";
  const failures = data.failures.map(failureLine);
  byId("failures").replaceChildren(...(failures.length ? failures
    : [emptyLine(data.runs ? "No failed executions." : "No executions yet. The rule engine records runs here once it is live.")]));
  const releases = data.releases.map(releaseLine);
  byId("releases").replaceChildren(...(releases.length ? releases : [emptyLine("No releases yet. Publishing creates the first release.")]));
}

async function refresh(): Promise<void> {
  setStatus(statusLine, "Loading overview…");
  try {
    const data = await loadOverview();
    const tables = [...new Set(data.rules.flatMap((r) => [r.live?.targetTable, r.draft?.targetTable]).filter((t): t is string => !!t))];
    const tableNames = await tableDisplayNames(tables);
    byId("tableCount").textContent = plural(tables.length, "table covered", "tables covered");
    renderMetrics(data, tables);
    renderLists(data, tableNames);
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the overview", error), "error");
  }
}

function start(): void {
  initWebApi();
  applyRole();
  byId("allDrafts").addEventListener("click", () => void go("rule-library", { status: "draft" }));
  void refresh();
}

start();
