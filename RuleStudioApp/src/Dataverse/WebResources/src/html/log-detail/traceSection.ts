// The trace section of the execution detail pane: one rs_trace document (json-formats.md §4)
// drawn with the shared trace renderer, or why there is nothing to draw.
import { h } from "../../shared/dom";
import { parseDocument } from "../../shared/formats";
import { changesTable, traceStep } from "../../shared/traceView";

import type { ExecutionTrace } from "../../shared/formats";

const UNREADABLE: Record<string, string> = {
  empty: "No trace was recorded for this run.",
  invalidJson: "The trace is not valid JSON.",
  unsupportedVersion: "The trace uses a format version this page does not know.",
};

function levelNote(trace: ExecutionTrace): HTMLElement | null {
  if (trace.level === "fullTrace") return null;
  return h("p", { className: "muted t11" },
    "Summary trace: conditions, actions and before/after values are kept only at the Full trace log level.");
}

export function renderTrace(target: HTMLElement, text: string | null): void {
  const parsed = parseDocument<ExecutionTrace>(text);
  if (!parsed.ok) {
    target.replaceChildren(h("div", { className: "scope-box" }, UNREADABLE[parsed.reason] ?? "The trace cannot be read."));
    return;
  }
  const trace = parsed.value;
  const parts: (HTMLElement | null)[] = [levelNote(trace), traceStep(trace)];
  if (trace.level === "fullTrace") {
    parts.push(h("h3", {}, "Before / after field values"), changesTable([trace]));
  }
  target.replaceChildren(...parts.filter((part): part is HTMLElement => part !== null));
}
