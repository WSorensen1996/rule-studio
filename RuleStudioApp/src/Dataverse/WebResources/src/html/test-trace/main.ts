// Test & trace page: runs rs_SimulateRule for a table/event/stage chain or selected rules,
// against a real record and/or sample values, and renders the returned traces.
// Params: ruleId or ruleIds (comma separated) | table, event, stage (choice values).
import { addSample, chosenRecordId, readSamples, resetRecord, searchRecords } from "./recordInput";
import { applyRole } from "../../shared/auth";
import { byId, h, pill } from "../../shared/dom";
import { tableDisplayNames } from "../../shared/metadata";
import { pageParams } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { simulate } from "../../shared/releaseApi";
import { applyStandard } from "../../shared/standard";
import { traceElements } from "../../shared/traceView";
import { initWebApi } from "../../shared/xrm";
import { loadChainRules } from "../execution-timeline/chainData";
import { buildChain, distinctTriggers, versionFor } from "../execution-timeline/chainModel";
import { applyTriggerParams, fillPickers, readTrigger } from "../execution-timeline/pickers";

import type { SimulateRequest } from "../../shared/releaseApi";
import type { ChainRule } from "../execution-timeline/chainData";
import type { ViewMode } from "../execution-timeline/chainModel";
import type { Pickers } from "../execution-timeline/pickers";

applyStandard();

const statusLine = byId("status");
const pickers: Pickers = {
  table: byId<HTMLSelectElement>("ttTable"),
  event: byId<HTMLSelectElement>("ttEvent"),
  stage: byId<HTMLSelectElement>("ttStage"),
};

let rules: ChainRule[] = [];
let tableNames = new Map<string, string>();
let scope: "chain" | "rules" = "chain";
const selected = new Set<string>();

function mode(): ViewMode {
  return byId<HTMLSelectElement>("versionMode").value === "live" ? "live" : "draft";
}

function refill(): void {
  fillPickers(pickers, distinctTriggers(rules), tableNames);
}

/** The table being tested: the chain's table, or the table of the first selected rule. */
function currentTable(): string {
  if (scope === "chain") return pickers.table.value;
  const rule = rules.find((r) => selected.has(r.id));
  return (rule && versionFor(rule, "draft")?.targetTable) ?? "";
}

// ── Chain preview and rule list ──────────────────────────────────────────

function previewLine(index: number, name: string, priority: number | null, draft: boolean): HTMLElement {
  return h("div", { className: "list-line" }, h("div", { className: "guide-n sm" }, String(index)),
    h("div", { className: "grow" }, h("b", { className: "t11" }, name), h("div", { className: "muted t11" }, `Priority ${priority ?? "—"}`)),
    draft ? pill("Draft", "draft") : pill("Live", "live"));
}

function renderPreview(): void {
  const current = mode();
  let lines: HTMLElement[];
  if (scope === "chain") {
    const trigger = readTrigger(pickers);
    const steps = trigger ? buildChain(rules, trigger, current).steps : [];
    lines = steps.map((s, i) => previewLine(i + 1, s.rule.name, s.version.priority, s.version.id === s.rule.draft?.id));
  } else {
    lines = rules.filter((r) => selected.has(r.id)).map((r, i) => {
      const version = versionFor(r, current);
      return previewLine(i + 1, r.name, version?.priority ?? null, version !== null && version.id === r.draft?.id);
    });
  }
  byId("preview").replaceChildren(...(lines.length ? lines : [h("p", { className: "muted t11" }, "No rules to run.")]));
}

function renderRuleList(): void {
  const filter = byId<HTMLInputElement>("ruleFilter").value.trim().toLowerCase();
  const shown = rules.filter((r) => versionFor(r, "draft") && (!filter || r.name.toLowerCase().includes(filter) || r.ruleKey.toLowerCase().includes(filter)));
  byId("ruleList").replaceChildren(...shown.map((rule) => {
    const box = h("input", { className: "check", attrs: { type: "checkbox" } });
    box.checked = selected.has(rule.id);
    box.addEventListener("change", () => {
      if (box.checked) selected.add(rule.id);
      else selected.delete(rule.id);
      onTableMaybeChanged();
      renderPreview();
    });
    const version = versionFor(rule, "draft");
    const trigger = version?.targetTable ? `${tableNames.get(version.targetTable) ?? version.targetTable}` : "no trigger";
    return h("label", {}, box, h("span", {}, rule.name, h("span", { className: "muted" }, ` · ${rule.ruleKey} · ${trigger}`)));
  }));
}

function setScope(next: "chain" | "rules"): void {
  scope = next;
  byId("scopeChain").setAttribute("aria-pressed", String(next === "chain"));
  byId("scopeRules").setAttribute("aria-pressed", String(next === "rules"));
  byId("chainPickers").hidden = next !== "chain";
  byId("rulePicker").hidden = next !== "rules";
  onTableMaybeChanged();
  renderPreview();
}

let sampleTable = "";

function onTableMaybeChanged(): void {
  const table = currentTable();
  if (table === sampleTable) return;
  sampleTable = table;
  resetRecord(table);
}

// ── Running ──────────────────────────────────────────────────────────────

function buildRequest(): SimulateRequest | string {
  const request: SimulateRequest = {
    useDrafts: mode() === "draft",
    mockFunctions: byId<HTMLInputElement>("mockAll").checked,
    sampleValues: readSamples(),
  };
  const recordId = chosenRecordId();
  if (recordId) request.recordId = recordId;
  if (scope === "rules") {
    if (selected.size === 0) return "Select at least one rule.";
    request.ruleIds = [...selected];
    return request;
  }
  const trigger = readTrigger(pickers);
  if (!trigger) return "Choose a table, event and stage.";
  return { ...request, table: trigger.table, event: String(trigger.event), stage: String(trigger.stage) };
}

async function run(): Promise<void> {
  const request = buildRequest();
  if (typeof request === "string") {
    setStatus(statusLine, request, "error");
    return;
  }
  const button = byId<HTMLButtonElement>("run");
  button.disabled = true;
  setStatus(statusLine, "Running simulation…");
  try {
    const traces = await simulate(request);
    byId("result").replaceChildren(...traceElements(traces, request.useDrafts !== false));
    byId("resultPill").hidden = false;
    setStatus(statusLine, "");
    toast("Simulation completed. Nothing was saved to Dataverse.");
  } catch (error) {
    setStatus(statusLine, describeError("run the simulation", error), "error");
  } finally {
    button.disabled = false;
  }
}

async function refresh(): Promise<void> {
  const button = byId<HTMLButtonElement>("refresh");
  button.disabled = true;
  setStatus(statusLine, "Loading rules…");
  try {
    rules = (await loadChainRules()).sort((a, b) => a.name.localeCompare(b.name));
    tableNames = await tableDisplayNames(distinctTriggers(rules).map((t) => t.table));
    refill();
    renderRuleList();
    onTableMaybeChanged();
    renderPreview();
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the rules", error), "error");
  } finally {
    button.disabled = false;
  }
}

function applyParams(): void {
  const params = pageParams();
  const ids = (params.ruleIds ?? params.ruleId ?? "").split(",").map((id) => id.trim().replace(/[{}]/g, "").toLowerCase()).filter(Boolean);
  if (ids.length) {
    for (const id of ids) selected.add(id);
    renderRuleList();
    setScope("rules");
    return;
  }
  applyTriggerParams(pickers, params, refill);
  refill();
  onTableMaybeChanged();
  renderPreview();
}

function start(): void {
  initWebApi();
  applyRole();
  for (const select of [pickers.table, pickers.event, pickers.stage]) {
    select.addEventListener("change", () => {
      refill();
      onTableMaybeChanged();
      renderPreview();
    });
  }
  byId("scopeChain").addEventListener("click", () => setScope("chain"));
  byId("scopeRules").addEventListener("click", () => setScope("rules"));
  byId("ruleFilter").addEventListener("input", renderRuleList);
  byId("versionMode").addEventListener("change", renderPreview);
  byId("recordFind").addEventListener("click", () => void searchRecords(currentTable()));
  byId("recordSearch").addEventListener("keydown", (event) => {
    if (event.key === "Enter") void searchRecords(currentTable());
  });
  byId("addSample").addEventListener("click", () => void addSample(currentTable(), statusLine));
  byId("run").addEventListener("click", () => void run());
  byId("refresh").addEventListener("click", () => void refresh());
  void refresh().then(applyParams);
}

start();
