// Typed input mapping for a registered function: one row per parameter in its input schema
// (json-formats.md §2). Inputs left empty are not stored.
import { TEXT_TARGET } from "./fixedValue";
import { newMapping } from "./model";
import { INPUT_SOURCES, sourceSelect, valueControl } from "./valueEditor";
import { h } from "../../shared/dom";

import type { Catalog, FunctionInfo } from "./catalog";
import type { ValueTarget } from "./fixedValue";
import type { MappingNode, Removed } from "./model";
import type { FunctionParameter } from "../../shared/formats";

export interface MappingContext {
  rootTable: string;
  catalog: Catalog;
  disabled: boolean;
  removed: Removed;
  onChange: (structural: boolean) => void;
}

function targetFor(parameter: FunctionParameter): ValueTarget {
  switch (parameter.type) {
    case "number": case "decimal": case "money": return { ...TEXT_TARGET, kind: "number" };
    case "boolean": return { ...TEXT_TARGET, kind: "boolean" };
    case "date": return { ...TEXT_TARGET, kind: "date" };
    case "lookup": return { ...TEXT_TARGET, kind: "lookup", targets: parameter.table ? [parameter.table] : [] };
    default: return TEXT_TARGET;
  }
}

/** Moves an owner's stored mappings to the removed list, e.g. when another function is chosen. */
export function clearMappings(mappings: MappingNode[], removed: Removed): void {
  for (const m of mappings) if (!m.isNew) removed.mappings.push(m.id);
  mappings.length = 0;
}

function mappingRow(mapping: MappingNode, parameter: FunctionParameter | undefined, context: MappingContext, remove: () => void): HTMLElement {
  const options = {
    value: mapping.value, rootTable: context.rootTable, target: () => Promise.resolve(parameter ? targetFor(parameter) : TEXT_TARGET),
    multiplicity: "one" as const, sources: INPUT_SOURCES, catalog: context.catalog, disabled: context.disabled,
    label: mapping.parameterName, onChange: context.onChange,
  };
  const title = parameter
    ? h("label", { className: "form-label" }, parameter.displayName ?? parameter.name,
      h("span", { className: "muted" }, ` · ${parameter.type}${parameter.table ? ` (${parameter.table})` : ""}${parameter.required ? " · required" : ""}`))
    : h("span", { className: "form-label" }, mapping.parameterName, h("span", { className: "pill warning" }, "Not in the function's inputs"));
  const line = h("div", { className: "mapping-line" }, sourceSelect(options), valueControl(options));
  if (!parameter) {
    const button = h("button", { className: "btn mini square", attrs: { type: "button", "aria-label": `Remove input ${mapping.parameterName}` } }, "×");
    button.disabled = context.disabled;
    button.addEventListener("click", remove);
    line.append(button);
  }
  const field = h("div", { className: "field" }, title, line);
  if (parameter?.description) field.append(h("div", { className: "hint" }, parameter.description));
  return field;
}

function functionNote(fn: FunctionInfo): HTMLElement {
  const notes = [`Custom API ${fn.apiName}. Returns success, value and message.`];
  if (fn.hasSideEffects) notes.push("⚠ Has side effects: mocked when testing.");
  if (!fn.safeToRetry) notes.push("Not retry-safe.");
  return h("p", { className: "muted t11 mapping-note" }, notes.join(" "));
}

/** The mapping grid for `fn`, adding (unsaved) rows for parameters that have no mapping yet. */
export function mappingEditor(fn: FunctionInfo | undefined, mappings: MappingNode[], context: MappingContext): HTMLElement {
  const box = h("div", { className: "action-details" });
  if (!fn) return box;
  box.append(h("b", {}, "Typed input mapping"));
  const parameters = fn.parameters ?? [];
  if (!fn.parameters) box.append(h("p", { className: "notice" }, "This function's input schema is missing or unreadable. Fix it on the Custom functions page."));
  for (const parameter of parameters) {
    if (!mappings.some((m) => m.parameterName === parameter.name)) mappings.push(newMapping(parameter.name));
  }
  const grid = h("div", { className: "field-grid mapping-grid" });
  for (const mapping of [...mappings]) {
    const parameter = parameters.find((p) => p.name === mapping.parameterName);
    grid.append(mappingRow(mapping, parameter, context, () => {
      if (!mapping.isNew) context.removed.mappings.push(mapping.id);
      mappings.splice(mappings.indexOf(mapping), 1);
      context.onChange(true);
    }));
  }
  if (parameters.length === 0 && mappings.length === 0) grid.append(h("p", { className: "muted t11" }, "This function takes no inputs."));
  box.append(grid);
  box.append(functionNote(fn));
  return box;
}
