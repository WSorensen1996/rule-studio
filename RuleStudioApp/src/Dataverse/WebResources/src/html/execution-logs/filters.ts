// The filter bar of the execution logs page: reads it into a LogFilter and fills it from page
// parameters (ruleId, correlationId, flagged=1, mode) so other pages can open a filtered view.
import { OUTCOME_OPTIONS } from "../../shared/data/executionLogs";
import { byId, fillSelect } from "../../shared/dom";

import type { LogFilter, LogMode, RuleOption } from "../../shared/data/executionLogs";
import type { Params } from "../../shared/nav";

const MODES: LogMode[] = ["all", "production", "test"];

const rule = byId<HTMLSelectElement>("fRule");
const table = byId<HTMLInputElement>("fTable");
const outcome = byId<HTMLSelectElement>("fOutcome");
const from = byId<HTMLInputElement>("fFrom");
const to = byId<HTMLInputElement>("fTo");
const mode = byId<HTMLSelectElement>("fMode");
const correlation = byId<HTMLInputElement>("fCorrelation");
const flagged = byId<HTMLInputElement>("fFlagged");

export function readFilter(): LogFilter {
  return {
    ruleId: rule.value,
    table: table.value.trim(),
    outcome: outcome.value,
    from: from.value,
    to: to.value,
    mode: MODES.find((m) => m === mode.value) ?? "all",
    flaggedOnly: flagged.checked,
    correlationId: correlation.value.trim(),
  };
}

export function fillOptions(rules: RuleOption[]): void {
  fillSelect(rule, [{ value: "", label: "All rules" }, ...rules.map((r) => ({ value: r.id, label: r.name }))]);
  fillSelect(outcome, [{ value: "", label: "Any outcome" }, { value: "errors", label: "Failures & blocks" }, ...OUTCOME_OPTIONS]);
}

/** Pre-selects a rule that may not be in the options yet (they load later). */
export function applyParams(params: Params): void {
  if (params.ruleId) {
    if (!Array.from(rule.options).some((o) => o.value === params.ruleId)) rule.append(new Option(params.ruleId, params.ruleId));
    rule.value = params.ruleId;
  }
  if (params.correlationId) correlation.value = params.correlationId;
  if (params.flagged === "1") flagged.checked = true;
  if (params.mode && MODES.some((m) => m === params.mode)) mode.value = params.mode;
}

export function setCorrelation(id: string): void {
  correlation.value = id;
}

export function clearFilters(): void {
  for (const input of [table, from, to, correlation]) input.value = "";
  rule.value = "";
  outcome.value = "";
  mode.value = "all";
  flagged.checked = false;
}
