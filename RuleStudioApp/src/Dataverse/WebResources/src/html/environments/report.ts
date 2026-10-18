// Renders an import report: issues, one row per rule key, lookups, and the side-by-side diff of conflicts.
import { h, pill } from "../../shared/dom";

import type { ImportReport, ImportRuleResult, ImportStatus } from "../../shared/promotionApi";

const STATUS: Record<ImportStatus, { label: string; kind: "live" | "info" | "muted" | "warning" | "bad" }> = {
  create: { label: "Create", kind: "live" },
  update: { label: "Update", kind: "live" },
  unchanged: { label: "Unchanged", kind: "muted" },
  keepTarget: { label: "Keep target", kind: "info" },
  targetOnly: { label: "Only here", kind: "info" },
  conflict: { label: "Conflict", kind: "bad" },
};

function ruleRow(rule: ImportRuleResult): HTMLTableRowElement {
  const status = STATUS[rule.status] ?? { label: rule.status, kind: "muted" };
  return h("tr", {},
    h("td", {}, h("div", { className: "table-name mono" }, rule.ruleKey), h("div", { className: "table-description" }, rule.name ?? "")),
    h("td", {}, pill(status.label, status.kind)),
    h("td", { className: "t12" }, rule.message ?? ""));
}

function pretty(value: unknown): string {
  return value === undefined || value === null ? "(no published version)" : JSON.stringify(value, null, 2);
}

function diff(rules: ImportRuleResult[]): HTMLElement {
  const conflicts = rules.filter((r) => r.status === "conflict");
  if (conflicts.length === 0) return h("div", {});
  return h("div", {}, h("h3", {}, "Conflicts"), ...conflicts.map((rule) => h("div", { className: "section-gap" },
    h("b", { className: "mono t12" }, rule.ruleKey),
    h("div", { className: "diff-grid" },
      h("div", {}, h("div", { className: "form-label" }, "Incoming (bundle)"), h("pre", {}, pretty(rule.incoming))),
      h("div", {}, h("div", { className: "form-label" }, "Current (this environment)"), h("pre", {}, pretty(rule.current)))))));
}

export function reportRows(report: ImportReport): { issues: HTMLElement; rules: HTMLTableRowElement[]; lookups: HTMLElement; diff: HTMLElement } {
  const issues = report.issues.length === 0
    ? h("div", {})
    : h("ul", { className: "issue-list pad" }, ...report.issues.map((issue) => h("li", { className: issue.severity }, `${issue.code}: ${issue.message}`)));
  const lookups = report.lookups.length === 0
    ? h("div", {})
    : h("div", { className: "t12" }, h("b", {}, "Lookup definitions: "), report.lookups.map((l) => `${l.name} (${l.status})`).join(", "));
  const rules = report.rules.length
    ? report.rules.map(ruleRow)
    : [h("tr", {}, h("td", { attrs: { colspan: "3" }, className: "muted t12" }, "The bundle has no rules and none exist here."))];
  return { issues, rules, lookups, diff: diff(report.rules) };
}
