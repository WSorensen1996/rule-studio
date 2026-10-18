// Renders rs_trace documents (json-formats.md §4) for test runs (test-trace) and execution logs
// (log-detail): a verdict, one step per rule with its condition tree and action results, and a
// before/after table of the fields that changed. Styles: trace-view.css.
import { h, pill } from "./dom";

import type {
  ActionTrace, ConditionGroupTrace, ConditionTrace, ExecutionTrace, FixedValue, FunctionTrace, LookupTrace,
} from "./formats";

const OPERATORS: Record<ConditionTrace["operator"], string> = {
  equals: "Equals", notEquals: "Does not equal", greaterThan: "Greater than", greaterOrEqual: "Greater or equal",
  lessThan: "Less than", lessOrEqual: "Less or equal", between: "Between", inList: "In list", contains: "Contains",
  startsWith: "Starts with", isEmpty: "Is empty", isNotEmpty: "Is not empty", changed: "Changed",
};

const AGGREGATES: Record<string, string> = {
  any: "Any", all: "All", noneOf: "None of", count: "Count", sum: "Sum", min: "Min", max: "Max", average: "Average",
};

const SKIPS: Record<string, string> = {
  ruleDisabled: "The rule is inactive.",
  columnsNotChanged: "None of the filtered columns changed.",
  stoppedByEarlierRule: "An earlier rule stopped processing or blocked the save.",
};

export function formatValue(value: FixedValue | undefined): string {
  if (value === undefined || value === null) return "(empty)";
  if (typeof value === "object") return value.name ? `${value.name} (${value.table})` : `${value.table} ${value.id}`;
  if (typeof value === "string") return `"${value}"`;
  return String(value);
}

function resultPill(result: boolean | null, notEvaluated?: boolean): HTMLSpanElement {
  if (notEvaluated || result === null) return pill("Not evaluated", "muted");
  return result ? pill("True", "live") : pill("False", "muted");
}

function functionText(fn: FunctionTrace): string {
  const failure = fn.timedOut ? "timed out" : "failed";
  const outcome = fn.result.success ? `→ ${formatValue(fn.result.value)}` : `${failure}: ${fn.result.message ?? "no message"}`;
  return `Function ${fn.customApiName}${fn.mocked ? " (mocked)" : ""} ${outcome} · ${fn.durationMs} ms`;
}

function lookupText(lookup: LookupTrace): string {
  const policy = lookup.policy ? ` · policy: ${lookup.policy}` : "";
  return `Lookup: ${lookup.matchCount} match${lookup.matchCount === 1 ? "" : "es"} → ${formatValue(lookup.value)}${policy}`;
}

function conditionLabel(condition: ConditionTrace): string {
  const path = condition.path
    ? [condition.path.table, ...condition.path.hops.map((hop) => hop.table), condition.path.column].join(" → ")
    : "Value";
  const aggregate = condition.aggregate && condition.aggregate !== "none" ? `${AGGREGATES[condition.aggregate] ?? condition.aggregate} · ` : "";
  const expected = condition.expected !== undefined ? ` ${formatValue(condition.expected)}` : "";
  return `${aggregate}${path} ${OPERATORS[condition.operator] ?? condition.operator}${expected}`;
}

function conditionLines(node: ConditionGroupTrace | ConditionTrace, depth: number): HTMLElement[] {
  const level = depth > 0 ? ` l${Math.min(depth, 3)}` : "";
  if (node.type === "group") {
    return [
      h("div", { className: `trace-line${level}` }, h("span", {}, `${node.operator.toUpperCase()} group`), resultPill(node.result, node.notEvaluated)),
      ...node.children.flatMap((child) => conditionLines(child, depth + 1)),
    ];
  }
  const counted = node.total === undefined ? formatValue(node.actual) : `${formatValue(node.actual)} of ${node.total}`;
  const actual = node.notEvaluated ? "" : ` · actual ${counted}`;
  const lines = [h("div", { className: `trace-line${level}` },
    h("span", {}, conditionLabel(node), h("span", { className: "muted" }, actual)), resultPill(node.result, node.notEvaluated))];
  const sub = depth + 1 > 3 ? " l3" : ` l${depth + 1}`;
  if (node.lookup) lines.push(h("div", { className: `trace-line muted t11${sub}` }, lookupText(node.lookup)));
  if (node.function) lines.push(h("div", { className: `trace-line muted t11${sub}` }, functionText(node.function)));
  return lines;
}

function actionText(action: ActionTrace): string {
  switch (action.type) {
    case "setField": return `Set ${action.field ?? "?"}: ${formatValue(action.before)} → ${formatValue(action.after)}`;
    case "blockSave": return `Blocked save: ${action.message ?? ""}`;
    case "logMessage": return `Log (${action.messageLevel ?? "info"}): ${action.message ?? ""}`;
    default: return action.message ? `Call function: ${action.message}` : "Call function";
  }
}

function actionLine(action: ActionTrace): HTMLElement {
  const status = action.status === "done" ? "" : ` [${action.status}]`;
  const element = h("div", { className: `t11${action.status === "failed" ? " trace-error" : ""}` }, `→ ${actionText(action)}${status}`);
  const notes = [action.function && functionText(action.function), action.lookup && lookupText(action.lookup)];
  for (const note of notes) if (note) element.append(h("div", { className: "muted t11 trace-line l1" }, note));
  return element;
}

function outcomeView(trace: ExecutionTrace): { dot: string; dotClass: string; badge: HTMLSpanElement } {
  switch (trace.outcome) {
    case "matched": return { dot: "✓", dotClass: "", badge: pill("Matched", "live") };
    case "blocked": return { dot: "!", dotClass: " block", badge: pill("Blocked", "bad") };
    case "failed": return { dot: "×", dotClass: " block", badge: pill("Failed", "bad") };
    case "skipped": return { dot: "–", dotClass: " no", badge: pill(trace.skipReason === "stoppedByEarlierRule" ? "Stopped" : "Skipped", "muted") };
    default: return { dot: "○", dotClass: " no", badge: pill("Not matched", "muted") };
  }
}

function ruleName(trace: ExecutionTrace): string {
  return trace.rule?.name ?? trace.rule?.ruleKey ?? `Rule ${trace.sequence}`;
}

function identityText(trace: ExecutionTrace): string {
  if (trace.rule?.runAs === "system") return " · as system";
  return trace.rule?.runAs === "callingUser" ? " · as calling user" : "";
}

function versionText(trace: ExecutionTrace): string {
  if (!trace.rule) return "";
  if (trace.rule.draft) return ` · #${trace.rule.priority} · draft${identityText(trace)}`;
  const version = trace.rule.versionNumber ? ` · v${trace.rule.versionNumber}` : "";
  return ` · #${trace.rule.priority}${version}${identityText(trace)}`;
}

function metaText(trace: ExecutionTrace): string {
  const meta = [`${trace.durationMs} ms`];
  if (trace.skipReason) meta.push(SKIPS[trace.skipReason] ?? trace.skipReason);
  if (trace.branch === "then") meta.push("Then branch");
  if (trace.branch === "else") meta.push("Else branch");
  return meta.join(" · ");
}

function detailElement(trace: ExecutionTrace): HTMLElement {
  const detail = h("div", { className: "trace-detail" });
  if (trace.conditions) detail.append(h("b", {}, "Condition evaluation"), ...conditionLines(trace.conditions, 0));
  if (trace.actions?.length) {
    if (trace.conditions) detail.append(h("div", { className: "divider" }));
    detail.append(h("b", {}, "Action results"), ...trace.actions.map(actionLine));
  }
  if (trace.error) detail.append(h("div", { className: "t11 trace-error" }, `Error ${trace.error.code}: ${trace.error.message}`));
  if (trace.test?.mockedFunctions.length) detail.append(h("div", { className: "muted t11" }, `Mocked functions: ${trace.test.mockedFunctions.join(", ")}`));
  if (trace.truncated) detail.append(h("div", { className: "muted t11" }, "The trace was truncated."));
  return detail;
}

/** One rule's step: outcome, timing, condition tree, actions and error. */
export function traceStep(trace: ExecutionTrace): HTMLElement {
  const view = outcomeView(trace);
  const body = h("div", {},
    h("div", { className: "step-head" }, h("div", {}, h("b", {}, ruleName(trace)), h("span", { className: "muted t11" }, versionText(trace))), view.badge),
    h("div", { className: "muted t11" }, metaText(trace)));
  const detail = detailElement(trace);
  if (detail.childElementCount > 0) body.append(detail);
  return h("div", { className: "trace-step" }, h("div", { className: `trace-dot${view.dotClass}`, attrs: { "aria-hidden": "true" } }, view.dot), body);
}

function verdictText(traces: ExecutionTrace[]): { badge: HTMLSpanElement; title: string; text: string; bad: boolean } {
  const blocked = traces.find((t) => t.outcome === "blocked");
  if (blocked) {
    const message = blocked.actions?.find((a) => a.type === "blockSave")?.message;
    return { badge: pill("Blocked", "bad"), title: "Save would be blocked", text: `${ruleName(blocked)} blocked the save${message ? `: ${message}` : "."}`, bad: true };
  }
  const failed = traces.find((t) => t.outcome === "failed");
  if (failed) return { badge: pill("Failed", "bad"), title: "A rule failed", text: `${ruleName(failed)}: ${failed.error?.message ?? "failed"}`, bad: true };
  return { badge: pill("Allowed", "live"), title: "Record would pass this chain", text: "Actions applied to the in-memory copy only. No Dataverse records were changed.", bad: false };
}

function verdict(traces: ExecutionTrace[], draft: boolean): HTMLElement {
  const total = traces.reduce((sum, t) => sum + t.durationMs, 0);
  const meta = `${traces.length} ${traces.length === 1 ? "rule" : "rules"} · ${total} ms · ${draft ? "drafts where available" : "published only"}`;
  const view = verdictText(traces);
  return h("div", { className: `sim-result${view.bad ? " block" : ""}` },
    h("div", { className: "step-head" }, view.badge, h("span", { className: "muted t11" }, meta)),
    h("h3", { className: "verdict-title" }, view.title),
    h("p", { className: "muted t12" }, view.text));
}

/** Before/after values of the fields the traces set, with the rule that set them last. */
export function changesTable(traces: ExecutionTrace[]): HTMLElement {
  const first = new Map<string, FixedValue | undefined>();
  const last = new Map<string, { after: FixedValue | undefined; by: string }>();
  for (const trace of traces) {
    for (const action of trace.actions ?? []) {
      if (action.type !== "setField" || action.status !== "done" || !action.field) continue;
      if (!first.has(action.field)) first.set(action.field, action.before);
      last.set(action.field, { after: action.after, by: ruleName(trace) });
    }
  }
  const rows = [...last].map(([field, change]) => h("tr", {},
    h("td", {}, h("code", {}, field)), h("td", {}, formatValue(first.get(field))),
    h("td", {}, h("b", {}, formatValue(change.after))), h("td", {}, change.by)));
  if (rows.length === 0) rows.push(h("tr", {}, h("td", { className: "muted", attrs: { colspan: "4" } }, "No fields changed in this run.")));
  return h("div", { className: "table-scroll" }, h("table", { className: "trace-table" },
    h("thead", {}, h("tr", {}, h("th", {}, "Field"), h("th", {}, "Before"), h("th", {}, "After"), h("th", {}, "Written by"))),
    h("tbody", {}, ...rows)));
}

/** The whole result section for one simulation run. */
export function traceElements(traces: ExecutionTrace[], draft: boolean): HTMLElement[] {
  if (traces.length === 0) return [h("div", { className: "empty" }, h("b", {}, "No rules ran"))];
  return [
    verdict(traces, draft),
    h("div", { className: "section-gap" }, h("h3", {}, "Rule-by-rule trace"), ...traces.map(traceStep)),
    h("div", { className: "divider" }),
    h("h3", {}, "Before / after field values"),
    changesTable(traces),
  ];
}
