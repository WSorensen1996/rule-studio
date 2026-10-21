// The configuration panel for the selected lookup definition.
import { criterionText, valueText } from "./criteriaText";
import { renderMap } from "./mapView";
import { MULTI_MATCH_OPTIONS, MultiMatch, NO_MATCH_OPTIONS, NoMatch, policyLabel } from "./policies";
import { h, pill } from "../../shared/dom";
import { findColumn, tableLabel } from "../../shared/tableMetadata";
import { decodeFixed } from "../rule-editor/fixedValue";

import type { LookupSummary } from "./lookupData";
import type { MapCriterion } from "./mapView";

function readonlyField(label: string, value: HTMLElement | string): HTMLElement {
  return h("div", { className: "field" }, h("div", { className: "form-label" }, label), h("div", { className: "input readonly" }, value));
}

function tableField(label: string, table: string, column?: string): HTMLElement {
  const value = h("span", { className: "mono" }, column ?? table ?? "—");
  const field = readonlyField(label, value);
  if (table) {
    const text = column ? findColumn(table, column).then((c) => c?.label ?? column) : tableLabel(table);
    void text.then((label) => {
      const logical = column ?? table;
      value.textContent = label === logical ? logical : `${label} (${logical})`;
    });
  }
  return field;
}

function criteriaBlock(lookup: LookupSummary): HTMLElement {
  const box = h("div", { className: "scope-box" });
  if (!lookup.criteria) {
    box.textContent = lookup.criteriaProblem || "No match criteria: every row of the table matches.";
    return box;
  }
  const record = h("div", { className: "muted t11" }, `Evaluated for: ${lookup.criteria.recordTable}`);
  void tableLabel(lookup.criteria.recordTable).then((label) => {
    record.textContent = `Evaluated for a ${label} record. All criteria must hold.`;
  });
  const list = h("ul", { className: "criteria-list" });
  const criteria = lookup.criteria.criteria;
  if (criteria.length === 0) list.append(h("li", {}, "No criteria: every row matches."));
  for (const criterion of criteria) {
    const line = h("li", {}, "…");
    void criterionText(criterion, lookup.lookupTable).then((text) => {
      line.textContent = text;
    });
    list.append(line);
  }
  box.append(record, list);
  return box;
}

function mapBlock(lookup: LookupSummary): HTMLElement {
  const criteria: MapCriterion[] = (lookup.criteria?.criteria ?? []).map((c) => ({
    operator: c.operator,
    columns: c.operator === "range" ? [c.minColumn, c.maxColumn] : [c.column],
    values: c.operator === "in" ? c.values : (c.operator === "isEmpty" ? [] : [c.value]),
    wildcard: c.operator !== "isEmpty" && c.emptyMatchesAll === true,
  }));
  return renderMap({ recordTable: lookup.criteria?.recordTable ?? "", lookupTable: lookup.lookupTable, returnColumn: lookup.returnColumn, criteria });
}

function policies(lookup: LookupSummary): HTMLElement {
  const noMatch = h("div", {}, h("div", { className: "form-label" }, "If no rows match"),
    pill(policyLabel(NO_MATCH_OPTIONS, lookup.noMatchPolicy), lookup.noMatchPolicy === NoMatch.Block ? "warning" : "info"));
  if (lookup.noMatchPolicy === NoMatch.Usedefault) {
    const text = h("div", { className: "muted t11" }, "Default: none");
    const value = decodeFixed(lookup.defaultValue)[0];
    if (value !== undefined) {
      void valueText({ type: "fixed", value }).then((shown) => {
        text.textContent = `Default: ${shown}`;
      });
    }
    noMatch.append(text);
  }
  const multi = h("div", {}, h("div", { className: "form-label" }, "If multiple rows match"),
    pill(policyLabel(MULTI_MATCH_OPTIONS, lookup.multiMatchPolicy), lookup.multiMatchPolicy === MultiMatch.Error ? "warning" : "info"));
  if (lookup.sortColumn) multi.append(h("div", { className: "muted t11" }, `Sort: ${lookup.sortColumn}`));
  return h("div", { className: "field-grid detail-section" }, noMatch, multi);
}

function usageText(usage: number): string {
  if (usage === 0) return "Not used by any condition or action yet.";
  return `Used in ${usage} place${usage === 1 ? "" : "s"} (conditions and actions, across all rule versions).`;
}

export function renderDetail(lookup: LookupSummary): HTMLElement {
  return h("div", { className: "card pad" },
    h("div", { className: "detail-heading" },
      h("div", { className: "rs-summary" }, h("p", { className: "rs-summary-text" }, "Lookup definition")),
      h("div", { className: "rs-title" }, h("h2", {}, lookup.name),
        lookup.description ? h("p", { className: "rs-desc" }, lookup.description) : null)),
    h("div", { className: "field-grid" },
      tableField("Table with the answers", lookup.lookupTable),
      tableField("Return column", lookup.lookupTable, lookup.returnColumn || "—")),
    h("h3", { className: "detail-section" }, "Match criteria"),
    criteriaBlock(lookup),
    h("h3", { className: "detail-section" }, "How the fields connect"),
    mapBlock(lookup),
    policies(lookup),
    h("p", { className: "muted t11 detail-section" }, usageText(lookup.usage)));
}
