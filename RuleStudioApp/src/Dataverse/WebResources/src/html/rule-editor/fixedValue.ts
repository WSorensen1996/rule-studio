// Fixed values: the typed input for one value, a list (In list) or a pair (Between), stored in
// rs_value as JSON (json-formats.md, Fixed values). Choices store their integer value.
import { h } from "../../shared/dom";
import { hostXrm } from "../../shared/xrm";

import type { FixedValue, RecordReference } from "../../shared/formats";
import type { ChoiceOption, ValueKind } from "../../shared/tableMetadata";

export type Multiplicity = "one" | "list" | "range";

/** What kind of value is expected: from the compared column, the field being set or a function input. */
export interface ValueTarget {
  kind: ValueKind;
  options: ChoiceOption[];
  targets: string[];
}

export const TEXT_TARGET: ValueTarget = { kind: "text", options: [], targets: [] };

function isReference(value: unknown): value is RecordReference {
  return typeof value === "object" && value !== null && "id" in value && "table" in value;
}

/** The stored JSON as a list of values. Text that is not JSON (typed before this format) is one string. */
export function decodeFixed(raw: string): FixedValue[] {
  if (!raw.trim()) return [];
  try {
    const value: unknown = JSON.parse(raw);
    return (Array.isArray(value) ? value : [value]) as FixedValue[];
  } catch {
    return [raw];
  }
}

function encode(values: FixedValue[], multiplicity: Multiplicity): string {
  if (values.length === 0 || values.every((v) => v === "" || v === null)) return "";
  return JSON.stringify(multiplicity === "one" ? values[0] : values);
}

/** Display text of one value. */
export function fixedText(value: FixedValue | undefined): string {
  if (value === undefined || value === null) return "";
  if (isReference(value)) return value.name ?? value.id;
  return String(value);
}

function scalar(text: string, kind: ValueKind): FixedValue {
  const trimmed = text.trim();
  if ((kind === "number" || kind === "choice") && trimmed !== "" && Number.isFinite(Number(trimmed))) return Number(trimmed);
  return trimmed;
}

type Emit = (raw: string) => void;

function choiceSelect(target: ValueTarget, values: FixedValue[], multiple: boolean, emit: Emit): HTMLSelectElement {
  const options = target.kind === "boolean"
    ? [{ value: 1, label: "Yes" }, { value: 0, label: "No" }]
    : target.options;
  const select = h("select", { className: "select", attrs: { "aria-label": "Value" } },
    ...(multiple ? [] : [h("option", { attrs: { value: "" } }, "— choose —")]),
    ...options.map((o) => h("option", { attrs: { value: String(o.value) } }, o.label)));
  select.multiple = multiple;
  const chosen = new Set(values.map((v) => (typeof v === "boolean" ? (v ? "1" : "0") : fixedText(v))));
  select.querySelectorAll("option").forEach((o) => {
    o.selected = chosen.has(o.value);
  });
  select.addEventListener("change", () => {
    const picked: FixedValue[] = [];
    select.querySelectorAll("option").forEach((o) => {
      if (o.selected && o.value !== "") picked.push(target.kind === "boolean" ? o.value === "1" : Number(o.value));
    });
    emit(encode(picked, multiple ? "list" : "one"));
  });
  return select;
}

function recordPicker(target: ValueTarget, values: FixedValue[], multiple: boolean, emit: Emit): HTMLElement {
  const names = values.map(fixedText).join(", ");
  const button = h("button", { className: "select path-button", attrs: { type: "button", "aria-label": "Choose record" } },
    names || "Choose record…");
  const xrm = hostXrm();
  button.disabled = !xrm || target.targets.length === 0;
  button.addEventListener("click", () => {
    void xrm?.Utility.lookupObjects({ entityTypes: target.targets, defaultEntityType: target.targets[0], allowMultiSelect: multiple })
      .then((picked) => {
        if (picked.length === 0) return;
        emit(encode(picked.map((p) => ({ table: p.entityType, id: p.id.replace(/[{}]/g, "").toLowerCase(), name: p.name })), multiple ? "list" : "one"));
      });
  });
  return button;
}

function textInput(kind: ValueKind, value: string, label: string): HTMLInputElement {
  const type = kind === "number" ? "number" : (kind === "date" ? "date" : "text");
  const input = h("input", { className: "input", attrs: { type, "aria-label": label } });
  input.value = value;
  return input;
}

function typedInputs(target: ValueTarget, values: FixedValue[], multiplicity: Multiplicity, emit: Emit): HTMLElement {
  if (multiplicity === "range") {
    const low = textInput(target.kind, fixedText(values[0]), "From");
    const high = textInput(target.kind, fixedText(values[1]), "To");
    const send = (): void => emit(encode([scalar(low.value, target.kind), scalar(high.value, target.kind)], "range"));
    low.addEventListener("change", send);
    high.addEventListener("change", send);
    return h("div", { className: "range-inputs" }, low, h("span", { className: "muted t11" }, "and"), high);
  }
  if (multiplicity === "list") {
    const input = textInput("text", values.map(fixedText).join(", "), "Values, comma-separated");
    input.placeholder = "Comma-separated values";
    input.addEventListener("change", () => emit(encode(input.value.split(",").filter((t) => t.trim()).map((t) => scalar(t, target.kind)), "list")));
    return input;
  }
  const input = textInput(target.kind, fixedText(values[0]), "Value");
  input.addEventListener("change", () => emit(encode([scalar(input.value, target.kind)], "one")));
  return input;
}

/** The input for a fixed value of the expected kind. */
export function fixedInput(target: ValueTarget, raw: string, multiplicity: Multiplicity, emit: Emit): HTMLElement {
  const values = decodeFixed(raw);
  const multiple = multiplicity === "list";
  if (target.kind === "boolean" || (target.kind === "choice" && target.options.length > 0)) return choiceSelect(target, values, multiple, emit);
  if (target.kind === "lookup") return recordPicker(target, values, multiple, emit);
  return typedInputs(target, values, multiplicity, emit);
}
