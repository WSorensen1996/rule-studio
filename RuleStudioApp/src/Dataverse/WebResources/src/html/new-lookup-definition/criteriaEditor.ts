// The match-criteria grid: one row per criterion with operator, lookup-table column(s), the
// record value to compare with and the "empty matches all" wildcard.
import { emptyValue, OPERATORS } from "./criteriaModel";
import { h } from "../../shared/dom";
import { fixedInput, TEXT_TARGET } from "../rule-editor/fixedValue";
import { pathPicker } from "../rule-editor/pathPicker";
import { disableAll } from "../rule-editor/valueEditor";

import type { CriterionDraft, Operator, SourceKind, ValueDraft } from "./criteriaModel";
import type { ColumnInfo } from "../../shared/tableMetadata";
import type { ValueTarget } from "../rule-editor/fixedValue";

export interface CriteriaContext {
  drafts: CriterionDraft[];
  recordTable: string;
  /** Columns of the lookup table; empty until a table is chosen. */
  columns: ColumnInfo[];
  disabled: boolean;
  /** Draws the grid again after a structural change. */
  redraw: () => void;
}

const SOURCES: { value: SourceKind; label: string }[] = [
  { value: "fieldPath", label: "Record field" },
  { value: "fixed", label: "Fixed" },
  { value: "expression", label: "Expression" },
];

function select(label: string, options: { value: string; label: string }[], current: string, pick: (value: string) => void): HTMLSelectElement {
  const element = h("select", { className: "select", attrs: { "aria-label": label } },
    ...options.map((o) => h("option", { attrs: { value: o.value } }, o.label)));
  element.value = current;
  element.addEventListener("change", () => pick(element.value));
  return element;
}

function columnSelect(ctx: CriteriaContext, label: string, current: string, pick: (value: string) => void): HTMLSelectElement {
  const options = [{ value: "", label: ctx.columns.length ? "— column —" : "Choose the lookup table first" },
    ...ctx.columns.map((c) => ({ value: c.logicalName, label: c.label }))];
  if (current && !ctx.columns.some((c) => c.logicalName === current)) options.push({ value: current, label: `⚠ ${current}` });
  return select(label, options, current, (value) => {
    pick(value);
    ctx.redraw();
  });
}

function targetOf(ctx: CriteriaContext, column: string): ValueTarget {
  const info = ctx.columns.find((c) => c.logicalName === column);
  return info ? { kind: info.kind, options: info.options, targets: info.targets } : TEXT_TARGET;
}

function valueControl(ctx: CriteriaContext, value: ValueDraft, target: ValueTarget, label: string): HTMLElement {
  if (value.source === "fieldPath") {
    return pathPicker({
      rootTable: ctx.recordTable, path: value.path, allowChild: false, disabled: ctx.disabled, label,
      onChange: (path) => {
        value.path = path;
        ctx.redraw();
      },
    });
  }
  if (value.source === "expression") {
    const input = h("input", { className: "input mono", attrs: { type: "text", "aria-label": label, placeholder: "[quantity] * 2" } });
    input.value = value.expression;
    input.addEventListener("change", () => {
      value.expression = input.value;
    });
    return input;
  }
  return fixedInput(target, value.raw, "one", (raw) => {
    value.raw = raw;
  });
}

function valueEditor(ctx: CriteriaContext, value: ValueDraft, target: ValueTarget, label: string): HTMLElement {
  const source = select(`${label} source`, SOURCES, value.source, (picked) => {
    Object.assign(value, emptyValue(), { source: picked as SourceKind });
    ctx.redraw();
  });
  return h("div", { className: "criterion-value" }, source, valueControl(ctx, value, target, label));
}

function inValues(ctx: CriteriaContext, draft: CriterionDraft, where: string): HTMLElement {
  const target = targetOf(ctx, draft.column);
  const lines = draft.values.map((value, i) => {
    const remove = h("button", { className: "btn mini", attrs: { type: "button", "aria-label": `Remove value ${i + 1}` } }, "×");
    remove.addEventListener("click", () => {
      draft.values.splice(i, 1);
      ctx.redraw();
    });
    return h("div", { className: "criterion-in-line" }, valueEditor(ctx, value, target, `${where} value ${i + 1}`), remove);
  });
  const add = h("button", { className: "btn mini", attrs: { type: "button" } }, "+ Value");
  add.addEventListener("click", () => {
    draft.values.push(emptyValue());
    ctx.redraw();
  });
  return h("div", { className: "criterion-in" }, ...lines, add);
}

function columnsPart(ctx: CriteriaContext, draft: CriterionDraft, where: string): HTMLElement {
  if (draft.operator !== "range") {
    return columnSelect(ctx, `${where} column`, draft.column, (v) => {
      draft.column = v;
    });
  }
  return h("div", { className: "criterion-range" },
    columnSelect(ctx, `${where} minimum column`, draft.minColumn, (v) => {
      draft.minColumn = v;
    }),
    columnSelect(ctx, `${where} maximum column`, draft.maxColumn, (v) => {
      draft.maxColumn = v;
    }));
}

function valuePart(ctx: CriteriaContext, draft: CriterionDraft, where: string): HTMLElement {
  if (draft.operator === "isEmpty") return h("span", { className: "muted t11" }, "Matches rows where the column is empty.");
  if (draft.operator === "in") return inValues(ctx, draft, where);
  const column = draft.operator === "range" ? draft.minColumn : draft.column;
  return valueEditor(ctx, draft.value, targetOf(ctx, column), `${where} value`);
}

function wildcard(draft: CriterionDraft, where: string): HTMLElement {
  if (draft.operator === "isEmpty") return h("span");
  const box = h("input", { className: "check", attrs: { type: "checkbox", "aria-label": `${where}: empty matches all` } });
  box.checked = draft.emptyMatchesAll;
  box.addEventListener("change", () => {
    draft.emptyMatchesAll = box.checked;
  });
  return h("label", { className: "criterion-wildcard t11", attrs: { title: "A lookup row whose column is empty matches any value (counts as a wildcard)." } },
    box, " Empty matches all");
}

function criterionRow(ctx: CriteriaContext, draft: CriterionDraft, index: number): HTMLElement {
  const where = `Criterion ${index + 1}`;
  const operator = select(`${where} operator`, OPERATORS, draft.operator, (value) => {
    draft.operator = value as Operator;
    ctx.redraw();
  });
  const remove = h("button", { className: "btn mini", attrs: { type: "button", "aria-label": `Remove ${where}` } }, "Remove");
  remove.addEventListener("click", () => {
    ctx.drafts.splice(index, 1);
    ctx.redraw();
  });
  const row = h("div", { className: "criterion-row" },
    columnsPart(ctx, draft, where), operator, valuePart(ctx, draft, where), wildcard(draft, where), remove);
  disableAll(row, ctx.disabled);
  return row;
}

export function renderCriteria(container: HTMLElement, ctx: CriteriaContext): void {
  const rows = ctx.drafts.map((draft, i) => criterionRow(ctx, draft, i));
  if (rows.length === 0) rows.push(h("p", { className: "muted t11" }, "No criteria: every row of the lookup table matches."));
  container.replaceChildren(...rows);
}
