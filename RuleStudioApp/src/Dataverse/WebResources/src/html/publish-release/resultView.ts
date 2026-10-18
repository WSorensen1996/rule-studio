// Renders a ValidationResult (docs/json-formats.md §3): outcome banner, then each rule with its
// cost level and issues. Codes are an open list, so any code is shown as given.
import { h, pill } from "../../shared/dom";

import type { CostLevel, ValidationIssue, ValidationOutcome, ValidationResult } from "../../shared/formats";

const RELEASE_KEY = "";

function banner(result: ValidationResult): HTMLDivElement {
  const errors = result.issues.filter((issue) => issue.severity === "error").length;
  const warnings = result.issues.length - errors;
  const texts: Record<ValidationOutcome, [string, string, string]> = {
    passed: ["ok", "Blocking checks passed", "No issues found. The release can be published."],
    passedWithWarnings: ["", `Passed with ${warnings} warning${warnings === 1 ? "" : "s"}`, "Review the warnings before publishing."],
    failed: ["bad", `Validation failed: ${errors} error${errors === 1 ? "" : "s"}`, "Fix the errors in the drafts below; nothing will publish until they pass."],
  };
  const [kind, title, text] = texts[result.outcome] ?? ["bad", `Outcome: ${String(result.outcome)}`, ""];
  return h("div", { className: `notice${kind ? ` ${kind}` : ""}` }, h("strong", {}, title), text);
}

function costPill(cost: CostLevel | undefined): HTMLSpanElement | null {
  if (!cost) return null;
  const kinds: Record<CostLevel, "live" | "warning" | "bad"> = { low: "live", medium: "warning", high: "bad" };
  return pill(`Cost: ${cost}`, kinds[cost] ?? "muted");
}

function issueLine(issue: ValidationIssue): HTMLDivElement {
  const severity = issue.severity === "error" ? pill("Error", "bad") : pill("Warning", "warning");
  const location = issue.location ? h("div", { className: "muted t11" }, `In ${issue.location.kind}`) : null;
  return h("div", { className: "issue-line" }, severity,
    h("code", { className: "issue-code mono t11" }, issue.code),
    h("div", { className: "grow" }, issue.message, location));
}

function section(title: string, subtitle: string, cost: CostLevel | undefined, issues: ValidationIssue[]): HTMLDivElement {
  const body = issues.length ? issues.map(issueLine) : [h("div", { className: "muted t11" }, "No issues.")];
  return h("div", { className: "result-rule" },
    h("div", { className: "result-rule-head" }, h("b", { className: "t12" }, title), h("span", { className: "muted t11 mono" }, subtitle), costPill(cost)),
    ...body);
}

/** Groups issues by rule key; issues without a known rule fall under the release itself. */
function groupIssues(result: ValidationResult): Map<string, ValidationIssue[]> {
  const known = new Set(result.rules.map((rule) => rule.ruleKey));
  const groups = new Map<string, ValidationIssue[]>();
  for (const issue of result.issues) {
    const key = issue.ruleKey && known.has(issue.ruleKey) ? issue.ruleKey : (issue.ruleKey ?? RELEASE_KEY);
    groups.set(key, [...(groups.get(key) ?? []), issue]);
  }
  return groups;
}

export function renderResult(container: HTMLElement, result: ValidationResult, names: Map<string, string>): void {
  const groups = groupIssues(result);
  const sections = result.rules.map((rule) =>
    section(names.get(rule.ruleKey) ?? rule.ruleKey, `${rule.ruleKey} · draft v${rule.versionNumber}`, rule.costLevel, groups.get(rule.ruleKey) ?? []));
  const listed = new Set(result.rules.map((rule) => rule.ruleKey));
  groups.forEach((issues, key) => {
    if (listed.has(key)) return;
    sections.push(key === RELEASE_KEY ? section("Release", "", undefined, issues) : section(names.get(key) ?? key, key, undefined, issues));
  });
  container.replaceChildren(banner(result), ...sections);
}
