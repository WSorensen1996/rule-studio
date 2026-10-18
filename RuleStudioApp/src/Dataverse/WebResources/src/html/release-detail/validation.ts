// Draws a release's validation result (json-formats.md §3): outcome and the issues found.
import { outcomePill, readValidation } from "../../shared/data/releases";
import { h, pill } from "../../shared/dom";

import type { ValidationIssue } from "../../shared/formats";

function issueLine(issue: ValidationIssue): HTMLElement {
  const severity = issue.severity === "error" ? pill("Error", "bad") : pill("Warning", "warning");
  return h("div", { className: "list-line issue" },
    h("div", { className: "grow" },
      h("div", { className: "issue-head" }, severity, h("code", { className: "t11" }, String(issue.code)),
        issue.ruleKey ? h("span", { className: "muted t11 mono" }, issue.ruleKey) : null),
      h("div", { className: "t12" }, issue.message)));
}

export function renderValidation(target: HTMLElement, text: string | null): void {
  const { result, problem } = readValidation(text);
  if (!result) {
    target.replaceChildren(h("p", { className: "muted t12" }, `${problem}: this release has no readable validation result.`));
    return;
  }
  const issues = Array.isArray(result.issues) ? result.issues : [];
  const checked = result.checkedOn ? new Date(result.checkedOn).toLocaleString() : "unknown time";
  target.replaceChildren(
    h("div", { className: "issue-head" }, outcomePill(text), h("span", { className: "muted t11" }, `Checked ${checked}`)),
    issues.length
      ? h("div", {}, ...issues.map(issueLine))
      : h("p", { className: "muted t12" }, "No issues were reported."));
}
