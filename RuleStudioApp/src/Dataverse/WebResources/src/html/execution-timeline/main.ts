import { loadChainRules } from "./chainData";
import { buildChain, distinctTriggers } from "./chainModel";
import { chainElements, otherElements } from "./chainView";
import { applyTriggerParams, fillPickers, readTrigger } from "./pickers";
import { applyRole } from "../../shared/auth";
import { byId } from "../../shared/dom";
import { tableDisplayNames } from "../../shared/metadata";
import { navigateToPage, openRecord, pageParams } from "../../shared/nav";
import { describeError, setStatus } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { ChainRule } from "./chainData";
import type { ChainStep, Trigger } from "./chainModel";
import type { Pickers } from "./pickers";

applyStandard();

const statusLine = byId("status");
const pickers: Pickers = {
  table: byId<HTMLSelectElement>("tlTable"),
  event: byId<HTMLSelectElement>("tlEvent"),
  stage: byId<HTMLSelectElement>("tlStage"),
};

let rules: ChainRule[] = [];
let triggers: Trigger[] = [];
let tableNames = new Map<string, string>();

async function openRule(step: ChainStep): Promise<void> {
  try {
    await openRecord("rule-editor", { ruleId: step.rule.id });
  } catch (error) {
    setStatus(statusLine, describeError("open the rule editor", error), "error");
  }
}

async function testChain(): Promise<void> {
  const trigger = readTrigger(pickers);
  if (!trigger) {
    setStatus(statusLine, "Pick a table, event and stage first.", "error");
    return;
  }
  try {
    await navigateToPage("test-trace", { table: trigger.table, event: String(trigger.event), stage: String(trigger.stage) });
  } catch (error) {
    setStatus(statusLine, describeError("open Test & trace", error), "error");
  }
}

function refill(): void {
  fillPickers(pickers, triggers, tableNames);
}

function renderOthers(others: ChainStep[], handlers: Parameters<typeof otherElements>[1]): void {
  byId("otherSection").hidden = others.length === 0;
  byId("otherTitle").textContent = "Published rules whose draft no longer uses this trigger:";
  byId("otherChain").replaceChildren(...otherElements(others, handlers));
}

function renderEmpty(shown: number): void {
  const empty = byId("emptyChain");
  empty.hidden = shown > 0;
  empty.textContent = triggers.length === 0
    ? "No rule versions have a trigger yet. Set a table, event and stage on a rule in the rule editor."
    : "No draft or published rules use this trigger.";
}

function render(): void {
  const trigger = readTrigger(pickers);
  // Drafts are shown in place of their published version and marked as drafts.
  const { steps, others } = trigger ? buildChain(rules, trigger, "draft") : { steps: [], others: [] };
  const handlers = { onOpen: (step: ChainStep) => void openRule(step) };
  byId("chain").replaceChildren(...chainElements(steps, handlers));
  byId("chainCount").textContent = `· ${steps.length} ${steps.length === 1 ? "step" : "steps"}`;
  renderOthers(others, handlers);
  renderEmpty(steps.length);
}

async function refresh(): Promise<void> {
  const button = byId<HTMLButtonElement>("refresh");
  button.disabled = true;
  setStatus(statusLine, "Loading rules…");
  try {
    rules = await loadChainRules();
    triggers = distinctTriggers(rules);
    tableNames = await tableDisplayNames(triggers.map((t) => t.table));
    refill();
    render();
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the execution timeline", error), "error");
  } finally {
    button.disabled = false;
  }
}

function onPickerChange(): void {
  refill();
  render();
}

function start(): void {
  initWebApi();
  applyRole();
  for (const select of [pickers.table, pickers.event, pickers.stage]) select.addEventListener("change", onPickerChange);
  byId("refresh").addEventListener("click", () => void refresh());
  byId("testChain").addEventListener("click", () => void testChain());
  void refresh().then(() => {
    applyTriggerParams(pickers, pageParams(), refill);
    refill();
    render();
  });
}

start();
