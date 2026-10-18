import { applyParams, clearFilters, fillOptions, readFilter, setCorrelation } from "./filters";
import { initRetention } from "./retention";
import { applyRole } from "../../shared/auth";
// Execution logs page (sitemap: Operations › Execution logs). Lists rs_executionlog rows, newest
// first, with filters; Inspect opens the log-detail side pane. Params: ruleId, correlationId,
// flagged=1, mode (see filters.ts).
import {
  durationText, flagPill, loadLogs, loadRuleOptions, logOutcomePill, modePill,
} from "../../shared/data/executionLogs";
import { byId, h } from "../../shared/dom";
import { navigateToPage, openRecord, pageParams } from "../../shared/nav";
import { describeError, setStatus } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { LogSummary } from "../../shared/data/executionLogs";

applyStandard();

const MAX_SHOWN = 500;
const statusLine = byId("status");

async function inspect(log: LogSummary): Promise<void> {
  try {
    await openRecord("log-detail", { logId: log.id });
  } catch (error) {
    setStatus(statusLine, describeError("open the execution detail", error), "error");
  }
}

function correlationCell(log: LogSummary): HTMLElement | string {
  if (!log.correlationId) return "—";
  const button = h("button", { className: "link-button mono t11", attrs: { type: "button", title: "Show every rule run of this save" } },
    `${log.correlationId.slice(0, 8)}…`);
  button.addEventListener("click", () => {
    setCorrelation(log.correlationId);
    void refresh();
  });
  return button;
}

function attemptsText(log: LogSummary): string {
  return log.attempts === null || log.attempts === 0 ? "—" : String(log.attempts);
}

function row(log: LogSummary): HTMLTableRowElement {
  const open = h("button", { className: "btn mini", attrs: { type: "button" } }, "Inspect");
  open.addEventListener("click", () => void inspect(log));
  const mode = h("div", { className: "btn-group" }, modePill(log), log.adminFlag ? flagPill(true) : null);
  return h("tr", {},
    h("td", {}, log.createdOn),
    h("td", {}, h("div", { className: "table-name" }, log.ruleName),
      h("div", { className: "muted t11 mono" }, log.recordId || "—")),
    h("td", {}, logOutcomePill(log.outcome)),
    h("td", {}, log.trigger),
    h("td", {}, durationText(log.durationMs)),
    h("td", {}, attemptsText(log)),
    h("td", {}, mode),
    h("td", {}, correlationCell(log)),
    h("td", {}, open));
}

async function refresh(): Promise<void> {
  const button = byId<HTMLButtonElement>("refresh");
  button.disabled = true;
  setStatus(statusLine, "Loading execution logs…");
  try {
    const logs = await loadLogs(readFilter());
    byId("logRows").replaceChildren(...logs.map(row));
    byId("emptyState").hidden = logs.length > 0;
    const capped = logs.length >= MAX_SHOWN ? ` (newest ${MAX_SHOWN}; narrow the filters to see older runs)` : "";
    byId("logCount").textContent = `${logs.length} ${logs.length === 1 ? "run" : "runs"}${capped}`;
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the execution logs", error), "error");
  } finally {
    button.disabled = false;
  }
}

async function loadOptions(): Promise<void> {
  try {
    const selected = readFilter().ruleId;
    fillOptions(await loadRuleOptions());
    if (selected) applyParams({ ruleId: selected });
  } catch (error) {
    setStatus(statusLine, describeError("load the rules for the filter", error), "error");
  }
}

function start(): void {
  initWebApi();
  initRetention(applyRole());
  fillOptions([]);
  applyParams(pageParams());
  byId("refresh").addEventListener("click", () => void refresh());
  byId("filters").addEventListener("submit", (event) => {
    event.preventDefault();
    void refresh();
  });
  byId("clearFilters").addEventListener("click", () => {
    clearFilters();
    void refresh();
  });
  byId("testRule").addEventListener("click", () => {
    navigateToPage("test-trace").catch((error: unknown) => setStatus(statusLine, describeError("open Test & trace", error), "error"));
  });
  void loadOptions();
  void refresh();
}

start();
