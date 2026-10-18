import { h, pill } from "../../shared/dom";
import { eventLabel, stageLabel } from "../../shared/labels";

import type { RuleSummary, VersionSummary } from "../../shared/data/rules";

export interface Filters {
  search: string;
  table: string;
  status: string;
}

/** The version whose trigger and priority the list shows: live, else the draft, else the latest one. */
export function shownVersion(rule: RuleSummary): VersionSummary | null {
  return rule.live ?? rule.draft ?? rule.latest;
}

function matchesStatus(rule: RuleSummary, status: string): boolean {
  switch (status) {
    case "draft": return rule.draft !== null;
    case "live": return rule.live !== null;
    case "unpublished": return rule.live === null;
    case "disabled": return !rule.enabled;
    default: return true;
  }
}

/** Tables a rule triggers on: the live one and, when it differs, the draft one. */
export function ruleTables(rule: RuleSummary): string[] {
  return [rule.live?.targetTable, rule.draft?.targetTable].filter((t): t is string => !!t);
}

function matchesText(rule: RuleSummary, search: string): boolean {
  const text = search.trim().toLowerCase();
  return !text || `${rule.name} ${rule.ruleKey}`.toLowerCase().includes(text);
}

export function matches(rule: RuleSummary, filters: Filters): boolean {
  return (!filters.table || ruleTables(rule).includes(filters.table))
    && matchesStatus(rule, filters.status)
    && matchesText(rule, filters.search);
}

function triggerText(version: VersionSummary | null, tableNames: Map<string, string>): [string, string] {
  if (!version?.targetTable) return ["—", "No trigger yet"];
  return [tableNames.get(version.targetTable) ?? version.targetTable, `${eventLabel(version.event)} · ${stageLabel(version.stage)}`];
}

function sameTrigger(a: VersionSummary, b: VersionSummary): boolean {
  return a.targetTable === b.targetTable && a.event === b.event && a.stage === b.stage && a.priority === b.priority;
}

function triggerCell(rule: RuleSummary, tableNames: Map<string, string>): HTMLTableCellElement {
  const [table, when] = triggerText(shownVersion(rule), tableNames);
  const cell = h("td", {}, h("span", { className: "strong" }, table), h("div", { className: "muted t11" }, when));
  if (rule.live && rule.draft && !sameTrigger(rule.live, rule.draft)) {
    const [draftTable, draftWhen] = triggerText(rule.draft, tableNames);
    cell.append(h("div", { className: "t11 trigger-draft" }, `Draft: ${draftTable} · ${draftWhen} · priority ${rule.draft.priority ?? "—"}`));
  }
  return cell;
}

/** Neither live nor draft: normally its only release was rolled back. */
function inactivePill(rule: RuleSummary): HTMLElement {
  return rule.latestRolledBack ? pill(`Rolled back v${rule.latest?.versionNumber ?? "?"}`, "warning") : pill("Not published", "muted");
}

function statusCell(rule: RuleSummary): HTMLTableCellElement {
  const cell = h("td");
  if (rule.live) cell.append(h("span", { className: "stack" }, pill(`Live v${rule.live.versionNumber ?? "?"}`, "live")));
  if (rule.draft) cell.append(h("span", { className: "stack" }, pill(`Draft v${rule.draft.versionNumber ?? "?"}`, "draft")));
  if (!rule.live && !rule.draft) cell.append(h("span", { className: "stack" }, inactivePill(rule)));
  if (!rule.enabled) cell.append(h("span", { className: "stack" }, pill("Inactive", "warning")));
  return cell;
}

export interface RowHandlers {
  author: boolean;
  selected: Set<string>;
  onOpen: (rule: RuleSummary) => void;
  onSelect: () => void;
}

export function ruleRow(rule: RuleSummary, tableNames: Map<string, string>, handlers: RowHandlers): HTMLTableRowElement {
  const check = h("input", { className: "check", attrs: { type: "checkbox", "aria-label": `Select ${rule.name}` } });
  check.checked = handlers.selected.has(rule.id);
  check.disabled = !handlers.author;
  check.addEventListener("change", () => {
    if (check.checked) handlers.selected.add(rule.id);
    else handlers.selected.delete(rule.id);
    handlers.onSelect();
  });
  const open = h("button", { className: "btn mini", attrs: { type: "button" } }, "Open →");
  open.addEventListener("click", () => handlers.onOpen(rule));
  return h("tr", {},
    h("td", {}, check),
    h("td", {}, h("div", { className: "table-name" }, rule.name), h("div", { className: "table-description mono" }, rule.ruleKey)),
    triggerCell(rule, tableNames),
    h("td", { className: "mono" }, String(shownVersion(rule)?.priority ?? "—")),
    statusCell(rule),
    h("td", {}, open),
  );
}
