// Editable grid of the function's input parameters, stored as rs_inputschema.
import { fillSelect, h } from "../../shared/dom";

import type { FunctionParameter, ParameterType } from "../../shared/formats";
import type { TableName } from "../../shared/tableMetadata";

export const TYPE_OPTIONS: { value: ParameterType; label: string }[] = [
  { value: "text", label: "Text" }, { value: "number", label: "Number" }, { value: "decimal", label: "Decimal" },
  { value: "money", label: "Money" }, { value: "boolean", label: "Boolean" }, { value: "date", label: "Date" },
  { value: "lookup", label: "Lookup" },
];

const NAME_PATTERN = /^[A-Za-z_][A-Za-z0-9_]*$/;

export class ParamGrid {
  private rows: FunctionParameter[] = [];
  private tables: TableName[] = [];

  constructor(private readonly body: HTMLElement, private readonly editable: boolean) {}

  set(parameters: FunctionParameter[]): void {
    this.rows = parameters.map((p) => ({ ...p }));
    this.render();
  }

  setTables(tables: TableName[]): void {
    this.tables = tables;
    this.render();
  }

  add(): void {
    this.rows.push({ name: "", type: "text", required: true });
    this.render();
    this.body.querySelector<HTMLInputElement>("tr:last-child input")?.focus();
  }

  /** The parameters as stored; optional keys left out when empty. */
  value(): FunctionParameter[] {
    return this.rows.map((row) => {
      const parameter: FunctionParameter = { name: row.name.trim(), type: row.type, required: row.required };
      if (row.displayName?.trim()) parameter.displayName = row.displayName.trim();
      if (row.type === "lookup" && row.table) parameter.table = row.table;
      if (row.description?.trim()) parameter.description = row.description.trim();
      return parameter;
    });
  }

  /** First problem with the grid, or null when it can be saved. */
  problem(): string | null {
    const names = new Set<string>();
    for (const row of this.value()) {
      if (!NAME_PATTERN.test(row.name)) return `Parameter name "${row.name}" must be the Custom API unique name (letters, digits, underscore).`;
      if (names.has(row.name.toLowerCase())) return `Parameter "${row.name}" is listed twice.`;
      names.add(row.name.toLowerCase());
      if (row.type === "lookup" && !row.table) return `Choose the table for lookup parameter "${row.name}".`;
    }
    return null;
  }

  private render(): void {
    this.body.replaceChildren(...this.rows.map((row, index) => this.rowElement(row, index)));
    if (this.rows.length === 0) {
      this.body.append(h("tr", {}, h("td", { className: "muted", attrs: { colspan: "5" } }, "No inputs. Add a parameter or pick a Custom API to copy its request parameters.")));
    }
  }

  private input(value: string, label: string, onInput: (text: string) => void): HTMLInputElement {
    const input = h("input", { className: "input", attrs: { "aria-label": label } });
    input.value = value;
    input.disabled = !this.editable;
    input.addEventListener("input", () => onInput(input.value));
    return input;
  }

  private typeSelect(row: FunctionParameter): HTMLSelectElement {
    const select = h("select", { className: "select", attrs: { "aria-label": `Type of ${row.name || "parameter"}` } });
    fillSelect(select, TYPE_OPTIONS);
    select.value = row.type;
    select.disabled = !this.editable;
    select.addEventListener("change", () => {
      row.type = select.value as ParameterType;
      this.render();
    });
    return select;
  }

  private tableSelect(row: FunctionParameter): HTMLSelectElement | string {
    if (row.type !== "lookup") return "—";
    const select = h("select", { className: "select", attrs: { "aria-label": `Table of ${row.name || "parameter"}` } });
    const options = this.tables.map((t) => ({ value: t.logicalName, label: `${t.label} (${t.logicalName})` }));
    if (row.table && !options.some((o) => o.value === row.table)) options.unshift({ value: row.table, label: row.table });
    fillSelect(select, [{ value: "", label: this.tables.length ? "Choose a table…" : "Loading tables…" }, ...options]);
    select.value = row.table ?? "";
    select.disabled = !this.editable;
    select.addEventListener("change", () => {
      row.table = select.value || undefined;
    });
    return select;
  }

  private rowElement(row: FunctionParameter, index: number): HTMLTableRowElement {
    const required = h("input", { className: "check", attrs: { type: "checkbox", "aria-label": `${row.name || "Parameter"} is required` } });
    required.checked = row.required;
    required.disabled = !this.editable;
    required.addEventListener("change", () => {
      row.required = required.checked;
    });
    const remove = h("button", { className: "btn mini", attrs: { type: "button", "aria-label": `Remove ${row.name || "parameter"}` } }, "✕");
    remove.disabled = !this.editable;
    remove.addEventListener("click", () => {
      this.rows.splice(index, 1);
      this.render();
    });
    return h("tr", {},
      h("td", {}, this.input(row.name, "Parameter name", (text) => { row.name = text; })),
      h("td", {}, this.typeSelect(row)),
      h("td", {}, required),
      h("td", {}, this.tableSelect(row)),
      h("td", {}, remove));
  }
}
