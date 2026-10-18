// Section chrome: foldable section headers (all folded when the page opens) with a one-line summary, and the section 1 trigger
// cards. The cards drive the (visually hidden) Event and Stage selects, so formView.ts keeps
// binding the same controls.
import { ActionType, Branch, countConditions, Logical, RuleEvent } from "./model";
import { byId } from "../../shared/dom";
import { eventLabel, stageLabel } from "../../shared/labels";
import { tableLabel } from "../../shared/tableMetadata";

import type { EditorState } from "./model";

const INTERACTIVE = "button, a, input, select, textarea, details, summary, label";

/** Click a section header to fold or unfold it. Clicks on controls in the header are ignored. */
export function bindSections(): void {
  document.querySelectorAll<HTMLElement>("#builder > .form-section > .section-title").forEach((title) => {
    title.addEventListener("click", (event) => {
      if ((event.target as HTMLElement).closest(INTERACTIVE)) return;
      title.parentElement?.classList.toggle("folded");
    });
  });
}

/** A trigger card sets its select and fires change, exactly as picking from the select would. */
export function bindTriggerCards(): void {
  document.querySelectorAll<HTMLButtonElement>("[data-card]").forEach((card) => {
    card.addEventListener("click", () => {
      const select = byId<HTMLSelectElement>(card.dataset.card ?? "");
      if (select.disabled || select.value === card.dataset.value) return;
      select.value = card.dataset.value ?? "";
      select.dispatchEvent(new Event("change"));
    });
  });
}

let labelToken = 0;

function syncTrigger(state: EditorState): void {
  const { version } = state;
  document.querySelectorAll<HTMLButtonElement>("[data-card]").forEach((card) => {
    const select = byId<HTMLSelectElement>(card.dataset.card ?? "");
    const value = card.dataset.card === "ruleEvent" ? version.event : version.stage;
    card.setAttribute("aria-pressed", String(Number(card.dataset.value) === value));
    card.disabled = select.disabled;
  });
  const token = ++labelToken;
  const table = version.targetTable;
  void (table ? tableLabel(table) : Promise.resolve("Choose a table")).then((label) => {
    if (token !== labelToken) return;
    byId("ruleTableLabel").textContent = label;
    byId("ruleTableIcon").textContent = table ? (label[0] ?? "?").toUpperCase() : "?";
    // The trigger summary names the table, so write it again now that the label is known.
    const summary = document.querySelector<HTMLElement>('[data-summary="editor-identity"]');
    if (summary) summary.textContent = triggerSummary(state);
  });
}

function selectedText(id: string): string {
  return byId<HTMLSelectElement>(id).selectedOptions[0]?.textContent ?? "";
}

const plural = (k: number, word: string): string => `${k} ${word}${k === 1 ? "" : "s"}`;

function aboutSummary({ rule }: EditorState): string {
  const parts = [rule.ruleKey || "No key"];
  if (rule.description) parts.push(rule.description);
  return parts.join(" · ");
}

function triggerSummary({ version }: EditorState): string {
  const columns = version.event === RuleEvent.Update && version.filteredColumns ? version.filteredColumns.split(",").join(", ") : "any column";
  return `${byId("ruleTableLabel").textContent ?? version.targetTable} · ${eventLabel(version.event)} · ${stageLabel(version.stage)} · ${columns}`;
}

function conditionsSummary({ version }: EditorState): string {
  const count = countConditions(version.root);
  if (count === 0) return "No conditions: the actions run on every triggering event";
  return `${plural(count, "condition")} · match ${version.root.operator === Logical.OR ? "any" : "all"}`;
}

function actionsSummary({ version }: EditorState): string {
  const then = version.actions.filter((a) => a.branch !== Branch.Else);
  const otherwise = version.actions.filter((a) => a.branch === Branch.Else);
  const blocks = then.some((a) => a.actionType === ActionType.Blocksave) ? " (blocks the save)" : "";
  return `Then: ${plural(then.length, "action")}${blocks} · Else: ${otherwise.length ? plural(otherwise.length, "action") : "nothing"}`;
}

function runtimeSummary({ version }: EditorState): string {
  const stop = version.stopProcessing ? " · stops later rules" : "";
  return `Priority ${version.priority} · on failure: ${selectedText("vFailure")} · runs as ${selectedText("vRunAs")} · retries ${version.retryCount} · log: ${selectedText("vLog")}${stop}`;
}

const SUMMARIES: Record<string, (state: EditorState) => string> = {
  "editor-about": aboutSummary,
  "editor-identity": triggerSummary,
  "editor-conditions": conditionsSummary,
  "editor-actions": actionsSummary,
  "editor-runtime": runtimeSummary,
  "editor-activity": () => "Runs, duration and trend from the retained execution logs",
};

/** Called on every render: trigger cards, table label and the folded-section summaries. */
export function renderSections(state: EditorState): void {
  syncTrigger(state);
  document.querySelectorAll<HTMLElement>("[data-summary]").forEach((element) => {
    const summary = SUMMARIES[element.dataset.summary ?? ""];
    element.textContent = summary ? summary(state) : "";
  });
}
