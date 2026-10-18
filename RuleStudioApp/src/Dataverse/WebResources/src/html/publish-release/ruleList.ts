// The checklist of rules the release can include: each rule's key, name, table and draft vs live.
import { h, pill } from "../../shared/dom";

import type { RuleSummary } from "../../shared/data/rules";

export interface Candidates {
  rules: RuleSummary[];
  selected: Set<string>;
}

/**
 * Rules to offer: explicit ids offer those rules, otherwise every rule with a draft.
 * Preselects the given ids, or all drafts when none are given.
 */
export function pickCandidates(all: RuleSummary[], ruleIds: string[]): Candidates {
  const wanted = new Set(ruleIds.map((id) => id.toLowerCase()));
  let rules: RuleSummary[];
  if (wanted.size) rules = all.filter((rule) => wanted.has(rule.id.toLowerCase()));
  else rules = all.filter((rule) => rule.draft !== null);
  const preselect = rules.filter((rule) => rule.draft !== null && (wanted.size === 0 || wanted.has(rule.id.toLowerCase())));
  return { rules, selected: new Set(preselect.map((rule) => rule.id)) };
}

function versions(rule: RuleSummary): HTMLSpanElement {
  const draft = rule.draft ? pill(`Draft v${rule.draft.versionNumber ?? "?"}`, "draft") : pill("No draft", "muted");
  const live = rule.live ? pill(`Live v${rule.live.versionNumber ?? "?"}`, "live") : pill("Not published", "warning");
  return h("span", {}, draft, h("span", { className: "version-arrow" }, " → "), live);
}

function ruleLine(rule: RuleSummary, candidates: Candidates, onChange: () => void): HTMLLabelElement {
  const box = h("input", { className: "check", attrs: { type: "checkbox", "data-author": "" } });
  box.checked = candidates.selected.has(rule.id);
  box.disabled = rule.draft === null;
  box.addEventListener("change", () => {
    if (box.checked) candidates.selected.add(rule.id);
    else candidates.selected.delete(rule.id);
    onChange();
  });
  const table = rule.draft?.targetTable ?? rule.live?.targetTable ?? "No table";
  const detail = rule.draft ? table : `${table} · nothing to publish: the rule has no saved draft`;
  return h("label", { className: `list-line rule-check${rule.draft ? "" : " unavailable"}` },
    box,
    h("div", { className: "grow" },
      h("b", { className: "t12" }, rule.name),
      h("div", { className: "muted t11 mono" }, rule.ruleKey),
      h("div", { className: "muted t11" }, detail)),
    versions(rule));
}

export function renderRuleList(container: HTMLElement, candidates: Candidates, onChange: () => void): void {
  if (!candidates.rules.length) {
    container.replaceChildren(h("p", { className: "muted t12" }, "No rule has a saved draft, so there is nothing to publish."));
    return;
  }
  container.replaceChildren(...candidates.rules.map((rule) => ruleLine(rule, candidates, onChange)));
}

/** Selected ids in a stable order, so the same selection always gives the same key. */
export function selectedIds(candidates: Candidates): string[] {
  return candidates.rules.filter((rule) => candidates.selected.has(rule.id)).map((rule) => rule.id);
}
