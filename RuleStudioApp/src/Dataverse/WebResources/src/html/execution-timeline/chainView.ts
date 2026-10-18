// Chain cards: one per step, with priority, state, draft marker and the stop cut-off.
import { h, pill } from "../../shared/dom";

import type { ChainStep, StepDiff } from "./chainModel";

export interface ViewHandlers {
  onOpen: (step: ChainStep) => void;
}

const DRAFT_LABELS: Record<StepDiff, string> = {
  none: "Draft",
  added: "Draft · new here",
  changed: "Draft · changed",
  removed: "Draft removes",
};

/** The step shows an unpublished draft rather than the published version. */
function isDraft(step: ChainStep): boolean {
  return step.rule.draft?.id === step.version.id && step.rule.live?.id !== step.version.id;
}

function statePill(step: ChainStep): HTMLSpanElement {
  if (!step.rule.enabled) return pill("Inactive", "muted");
  if (step.cutOff) return pill("Not reached", "warning");
  if (step.diff === "removed") return pill(DRAFT_LABELS.removed, "bad");
  // A draft does not run yet, so it is marked as a draft rather than as enabled.
  if (isDraft(step)) return pill(step.rule.live ? DRAFT_LABELS[step.diff] : DRAFT_LABELS.none, "draft");
  return pill("Enabled", "live");
}

function versionText(step: ChainStep): string {
  const kind = isDraft(step) ? " (draft)" : "";
  return `v${step.version.versionNumber ?? "?"}${kind}`;
}

function detail(step: ChainStep): HTMLSpanElement {
  const columns = step.version.filteredColumns.trim() || "Any column";
  return h("span", { className: "chain-detail block" },
    h("span", { className: "block" }, h("b", {}, "Filtered columns: "), columns),
    h("span", { className: "block mono t11" }, step.rule.ruleKey),
    step.version.stopProcessing ? h("span", { className: "block chain-pills" }, pill("Stop processing", "info")) : null);
}

function stepClasses(step: ChainStep): string {
  const classes = ["chain-card"];
  if (!step.active) classes.push("is-inactive");
  if (step.cutOff) classes.push("is-cutoff");
  if (step.diff === "removed") classes.push("is-removed");
  else if (isDraft(step)) classes.push("is-diff");
  return classes.join(" ");
}

function card(step: ChainStep, handlers: ViewHandlers): HTMLButtonElement {
  const priority = step.version.priority === null ? "#—" : `#${step.version.priority}`;
  const element = h("button", { className: stepClasses(step), attrs: { type: "button", title: "Open in the rule editor" } },
    h("span", { className: "chain-top" }, pill(priority, "draft"), statePill(step)),
    h("strong", {}, step.rule.name),
    h("span", { className: "chain-meta block" }, versionText(step)),
    detail(step));
  element.addEventListener("click", () => handlers.onOpen(step));
  return element;
}

function cutMarker(): HTMLDivElement {
  return h("div", { className: "chain-cut", attrs: { role: "separator", "aria-label": "Stop processing: later rules are not reached" } },
    h("span", {}, "■ Stop"));
}

function connector(previous: ChainStep | undefined, step: ChainStep): HTMLDivElement {
  const stopsHere = previous !== undefined && !previous.cutOff && step.cutOff;
  return stopsHere ? cutMarker() : h("div", { className: "chain-arrow", attrs: { "aria-hidden": "true" } }, "→");
}

/** Cards joined by arrows, with a stop marker where processing ends. */
export function chainElements(steps: ChainStep[], handlers: ViewHandlers): HTMLElement[] {
  const out: HTMLElement[] = [];
  steps.forEach((step, index) => {
    if (index > 0) out.push(connector(steps[index - 1], step));
    out.push(card(step, handlers));
  });
  return out;
}

/** Dashed cards for published rules that their draft moves off this trigger. */
export function otherElements(steps: ChainStep[], handlers: ViewHandlers): HTMLElement[] {
  return steps.map((step) => card(step, handlers));
}
