// Execution detail page. Params: logId. Shows one rs_executionlog row: header and run
// metadata, the error, the rendered rs_trace (shared/traceView.ts), a link to the rule version
// and the other rule runs of the same save (same correlation ID), each opened as its own page.
import { XrmQuery } from "@delegateas/xrmquery";

import { renderTrace } from "./traceSection";
import { applyRole } from "../../shared/auth";
import {
  durationText, flagPill, loadCorrelated, loadLog, logOutcomePill, modePill,
} from "../../shared/data/executionLogs";
import { byId, h } from "../../shared/dom";
import { openRecord, pageParams } from "../../shared/nav";
import { describeError, setStatus } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { LogDetail, LogSummary } from "../../shared/data/executionLogs";

applyStandard();

const statusLine = byId("status");
let current: LogDetail | null = null;
let author = false;

function attemptsText(log: LogDetail): string {
  if (log.attempts === null || log.attempts === 0) return "Did not run";
  return log.attempts === 1 ? "1 attempt" : `${log.attempts} attempts (retried)`;
}

function showHeader(log: LogDetail): void {
  byId("lgRule").textContent = log.ruleName;
  document.title = `${log.ruleName} · Rule Studio`;
  byId("lgVersion").textContent = log.versionName;
  byId("lgOutcome").replaceChildren(logOutcomePill(log.outcome));
  byId("lgRecord").textContent = log.recordId ? `${log.table} ${log.recordId}` : `${log.table || "—"} (new record)`;
  byId("lgDuration").textContent = durationText(log.durationMs);
  byId("lgTime").textContent = log.createdOn;
  byId("lgStage").textContent = log.trigger;
  byId("lgCorr").textContent = log.correlationId || "—";
  byId("lgAttempts").textContent = attemptsText(log);
  byId("lgFlag").replaceChildren(flagPill(log.adminFlag));
  byId("lgLevel").textContent = log.logLevel;
  byId("lgMode").replaceChildren(modePill(log));
  byId("lgErrorBox").hidden = !log.errorMessage;
  byId("lgError").textContent = log.errorMessage;
  byId<HTMLButtonElement>("openRule").disabled = !log.ruleId;
  byId("clearFlag").hidden = !log.adminFlag;
}

// An admin reviewed the after-save failure: the flag goes, the log row stays.
async function clearFlag(): Promise<void> {
  if (!current) return;
  const button = byId<HTMLButtonElement>("clearFlag");
  button.disabled = true;
  try {
    await XrmQuery.update((x) => x.rs_executionlogs, current.id, { rs_adminflag: false }).promise();
    await load(current.id);
  } catch (error) {
    setStatus(statusLine, describeError("clear the admin flag", error), "error");
  } finally {
    button.disabled = !author;
  }
}

function relatedLine(log: LogSummary): HTMLElement {
  const open = h("button", { className: "btn mini", attrs: { type: "button" } }, "Open");
  open.addEventListener("click", () => void openRecord("log-detail", { logId: log.id }));
  return h("div", { className: "list-line" },
    h("div", { className: "grow" }, h("b", { className: "t12" }, log.ruleName),
      h("div", { className: "muted t11" }, `${log.trigger} · ${durationText(log.durationMs)}`)),
    h("div", { className: "related-pills" }, logOutcomePill(log.outcome)),
    open);
}

async function showRelated(log: LogDetail): Promise<void> {
  const target = byId("lgRelated");
  if (!log.correlationId) {
    target.replaceChildren(h("p", { className: "muted t12" }, "No correlation ID was recorded."));
    return;
  }
  const related = await loadCorrelated(log.correlationId, log.id);
  target.replaceChildren(...(related.length
    ? related.map(relatedLine)
    : [h("p", { className: "muted t12" }, "No other rule of this save wrote a log row.")]));
}

async function load(logId: string): Promise<void> {
  setStatus(statusLine, "Loading execution log…");
  try {
    const log = await loadLog(logId);
    current = log;
    showHeader(log);
    renderTrace(byId("lgTrace"), log.trace);
    await showRelated(log);
    byId("detail").hidden = false;
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the execution log", error), "error");
  }
}

async function openRule(): Promise<void> {
  if (!current?.ruleId) return;
  const params: Record<string, string> = { ruleId: current.ruleId };
  if (current.ruleVersionId) params.versionId = current.ruleVersionId;
  try {
    await openRecord("rule-editor", params);
  } catch (error) {
    setStatus(statusLine, describeError("open the rule editor", error), "error");
  }
}

function start(): void {
  initWebApi();
  author = applyRole();
  byId("clearFlag").addEventListener("click", () => void clearFlag());
  byId("openRule").addEventListener("click", () => void openRule());
  const { logId } = pageParams();
  if (!logId) {
    setStatus(statusLine, "Open execution details from the Execution logs page.", "error");
    return;
  }
  void load(logId);
}

start();
