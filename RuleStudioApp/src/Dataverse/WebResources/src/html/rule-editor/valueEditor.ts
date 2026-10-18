// Value source editor shared by conditions, actions and function inputs: a source select
// (Fixed / Field path / Expression / Lookup / Function) and the matching input.
import { fixedInput } from "./fixedValue";
import { SourceType } from "./model";
import { pathPicker } from "./pathPicker";
import { h } from "../../shared/dom";

import type { Catalog } from "./catalog";
import type { Multiplicity, ValueTarget } from "./fixedValue";
import type { ValueModel } from "./model";

const SOURCE_LABELS: Record<number, string> = {
  [SourceType.Fixed]: "Fixed",
  [SourceType.Fieldpath]: "Field path",
  [SourceType.Expression]: "Expression",
  [SourceType.Lookup]: "Lookup",
  [SourceType.Function]: "Function",
};

export const ALL_SOURCES = [SourceType.Fixed, SourceType.Fieldpath, SourceType.Expression, SourceType.Lookup, SourceType.Function];
/** Function inputs cannot call another function. */
export const INPUT_SOURCES = [SourceType.Fixed, SourceType.Fieldpath, SourceType.Expression, SourceType.Lookup];

export interface ValueEditorOptions {
  value: ValueModel;
  rootTable: string;
  target: () => Promise<ValueTarget>;
  multiplicity: Multiplicity;
  sources: number[];
  catalog: Catalog;
  disabled: boolean;
  label: string;
  /** structural = the row must be drawn again (another source or function was chosen). */
  onChange: (structural: boolean) => void;
}

/** Disables every control inside an element (viewer, or a read-only state). */
export function disableAll(element: HTMLElement, disabled: boolean): void {
  if (!disabled) return;
  element.querySelectorAll<HTMLInputElement | HTMLSelectElement | HTMLButtonElement | HTMLTextAreaElement>("input, select, button, textarea")
    .forEach((control) => {
      control.disabled = true;
    });
  if (element instanceof HTMLInputElement || element instanceof HTMLSelectElement || element instanceof HTMLButtonElement) element.disabled = true;
}

export function sourceSelect(options: ValueEditorOptions): HTMLSelectElement {
  const select = h("select", { className: "select", attrs: { "aria-label": `${options.label} value source`, "data-help": "valueSource" } },
    ...options.sources.map((s) => h("option", { attrs: { value: String(s) } }, SOURCE_LABELS[s] ?? String(s))));
  select.value = String(options.value.sourceType);
  select.disabled = options.disabled;
  select.addEventListener("change", () => {
    Object.assign(options.value, { sourceType: Number(select.value), value: "", valuePath: null, lookupDefinitionId: null, customFunctionId: null });
    options.onChange(true);
  });
  return select;
}

function pickSelect(label: string, items: { id: string; text: string }[], current: string | null, pick: (id: string | null) => void): HTMLSelectElement {
  const select = h("select", { className: "select", attrs: { "aria-label": label } },
    h("option", { attrs: { value: "" } }, items.length ? "— choose —" : "None available"),
    ...items.map((item) => h("option", { attrs: { value: item.id } }, item.text)));
  select.value = current ?? "";
  select.addEventListener("change", () => pick(select.value || null));
  return select;
}

function lookupSelect(options: ValueEditorOptions): HTMLSelectElement {
  const { value, catalog, rootTable } = options;
  const items = catalog.lookups
    .filter((l) => l.recordTable === rootTable || l.id === value.lookupDefinitionId)
    .map((l) => ({ id: l.id, text: l.recordTable === rootTable ? l.name : `⚠ ${l.name} (for ${l.recordTable ?? "unknown table"})` }));
  return pickSelect(`${options.label} lookup`, items, value.lookupDefinitionId, (id) => {
    value.lookupDefinitionId = id;
    options.onChange(false);
  });
}

function functionSelect(options: ValueEditorOptions): HTMLSelectElement {
  const { value, catalog } = options;
  const items = catalog.functions
    .filter((f) => f.enabled || f.id === value.customFunctionId)
    .map((f) => ({ id: f.id, text: f.enabled ? f.name : `⚠ ${f.name} (inactive)` }));
  return pickSelect(`${options.label} function`, items, value.customFunctionId, (id) => {
    value.customFunctionId = id;
    options.onChange(true);
  });
}

function expressionInput(options: ValueEditorOptions): HTMLInputElement {
  const input = h("input", { className: "input mono", attrs: { type: "text", "aria-label": `${options.label} expression`, placeholder: "[totalamount] * 0.1" } });
  input.value = options.value.value;
  input.addEventListener("change", () => {
    options.value.value = input.value;
    options.onChange(false);
  });
  return input;
}

function fixedControl(options: ValueEditorOptions): HTMLElement {
  const holder = h("div", { className: "value-holder" }, h("span", { className: "muted t11" }, "Loading…"));
  void options.target().then((target) => {
    holder.replaceChildren(fixedInput(target, options.value.value, options.multiplicity, (raw) => {
      options.value.value = raw;
      options.onChange(false);
    }));
    disableAll(holder, options.disabled);
  });
  return holder;
}

/** The input for the chosen source. */
export function valueControl(options: ValueEditorOptions): HTMLElement {
  let control: HTMLElement;
  switch (options.value.sourceType) {
    case SourceType.Fieldpath:
      control = pathPicker({
        rootTable: options.rootTable, path: options.value.valuePath, allowChild: false, disabled: options.disabled,
        label: `${options.label} source field`,
        onChange: (path) => {
          options.value.valuePath = path;
          options.onChange(true);
        },
      });
      break;
    case SourceType.Expression: control = expressionInput(options); break;
    case SourceType.Lookup: control = lookupSelect(options); break;
    case SourceType.Function: control = functionSelect(options); break;
    default: return fixedControl(options);
  }
  disableAll(control, options.disabled);
  return control;
}
