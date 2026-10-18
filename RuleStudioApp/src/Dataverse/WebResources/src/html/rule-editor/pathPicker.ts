// Field-path picker: a button showing the current path and a pop-over that builds one hop at a
// time. Each step lists the table's columns, its lookups (parent hops) and, when allowed, the
// tables that point back to it (child hops).
import { describePath } from "./paths";
import { h } from "../../shared/dom";
import { loadTable, tableLabel } from "../../shared/tableMetadata";

import type { FieldPath, FieldPathHop } from "../../shared/formats";
import type { TableInfo } from "../../shared/tableMetadata";

const MAX_HOPS = 4;

export interface PathPickerOptions {
  rootTable: string;
  path: FieldPath | null;
  /** Child hops make the path return a set; only conditions (with an aggregate) allow them. */
  allowChild: boolean;
  disabled: boolean;
  label: string;
  onChange: (path: FieldPath | null) => void;
}

function option(value: string, text: string, selected: boolean): HTMLOptionElement {
  const element = h("option", { attrs: { value } }, text);
  element.selected = selected;
  return element;
}

async function stepOptions(table: TableInfo, depth: number, allowChild: boolean, current: string): Promise<HTMLElement[]> {
  const columns = h("optgroup", { attrs: { label: "Columns" } },
    ...table.columns.map((c) => option(`col|${c.logicalName}`, c.label, current === `col|${c.logicalName}`)));
  const groups: HTMLElement[] = [option("", "— choose —", current === ""), columns];
  if (depth >= MAX_HOPS) return groups;
  const parents = table.columns.flatMap((c) => c.targets.map((target) => ({ column: c, target })));
  const parentLabels = await Promise.all(parents.map((p) => tableLabel(p.target)));
  groups.push(h("optgroup", { attrs: { label: "Related record (lookup)" } }, ...parents.map((p, i) => {
    const value = `parent|${p.column.logicalName}|${p.target}`;
    const suffix = p.column.targets.length > 1 ? ` (${parentLabels[i]})` : "";
    return option(value, `→ ${p.column.label}${suffix}`, current === value);
  })));
  if (allowChild) {
    const childLabels = await Promise.all(table.children.map((c) => tableLabel(c.table)));
    groups.push(h("optgroup", { attrs: { label: "Child records" } }, ...table.children.map((c, i) => {
      const value = `child|${c.attribute}|${c.table}`;
      return option(value, `⇉ ${childLabels[i]} (${c.attribute})`, current === value);
    })));
  }
  return groups;
}

function hopValue(hop: FieldPathHop): string {
  return `${hop.kind}|${hop.attribute}|${hop.table}`;
}

class PathBuilder {
  readonly panel = h("div", { className: "path-builder", attrs: { role: "dialog" } });
  private hops: FieldPathHop[] = [];
  private column = "";

  constructor(private readonly options: PathPickerOptions, private readonly commit: (path: FieldPath | null) => void) {}

  open(): void {
    this.hops = [...(this.options.path?.hops ?? [])];
    this.column = this.options.path?.column ?? "";
    this.panel.hidden = false;
    void this.render();
  }

  close(): void {
    this.panel.hidden = true;
  }

  private tableAt(depth: number): string {
    return depth === 0 ? this.options.rootTable : (this.hops[depth - 1]?.table ?? "");
  }

  private async render(): Promise<void> {
    const steps: HTMLElement[] = [];
    try {
      for (let depth = 0; depth <= this.hops.length; depth++) {
        const table = await loadTable(this.tableAt(depth));
        const hop = this.hops[depth];
        const current = hop ? hopValue(hop) : (this.column ? `col|${this.column}` : "");
        const select = h("select", { className: "select", attrs: { "aria-label": `${table.label}: column or related table` } },
          ...await stepOptions(table, depth, this.options.allowChild, current));
        select.addEventListener("change", () => this.choose(depth, select.value));
        steps.push(h("div", { className: "path-step" }, h("span", { className: "micro-label" }, table.label.toUpperCase()), select));
      }
    } catch {
      steps.push(h("p", { className: "notice bad" }, `Table "${this.tableAt(steps.length)}" was not found. Check the target table.`));
    }
    const clear = h("button", { className: "btn mini", attrs: { type: "button" } }, "Clear");
    clear.addEventListener("click", () => this.commit(null));
    const done = h("button", { className: "btn mini", attrs: { type: "button" } }, "Close");
    done.addEventListener("click", () => this.close());
    this.panel.replaceChildren(...steps, h("div", { className: "btn-group path-actions" }, clear, done));
    steps.at(-1)?.querySelector("select")?.focus();
  }

  private choose(depth: number, value: string): void {
    const [kind, name = "", table = ""] = value.split("|");
    this.hops = this.hops.slice(0, depth);
    this.column = "";
    if (kind === "col") {
      this.commit({ table: this.options.rootTable, hops: this.hops, column: name });
      return;
    }
    if (kind === "parent" || kind === "child") this.hops.push({ kind, attribute: name, table });
    void this.render();
  }
}

/** The picker: a summary button plus its pop-over. */
export function pathPicker(options: PathPickerOptions): HTMLElement {
  const button = h("button", { className: "select path-button", attrs: { type: "button", "aria-label": options.label } },
    options.path ? "…" : "Choose a field…");
  button.disabled = options.disabled || !options.rootTable;
  if (!options.rootTable) button.textContent = "Set the target table first";
  if (options.path) {
    const path = options.path;
    void describePath(path).then((text) => {
      button.textContent = path.table === options.rootTable ? text : `⚠ ${text} (starts at ${path.table})`;
      button.title = button.textContent;
    });
  }
  const builder = new PathBuilder(options, (path) => {
    builder.close();
    options.onChange(path);
  });
  builder.close();
  button.addEventListener("click", () => (builder.panel.hidden ? builder.open() : builder.close()));
  const field = h("div", { className: "path-field" }, button, builder.panel);
  field.addEventListener("keydown", (event) => {
    if (event.key === "Escape") builder.close();
  });
  return field;
}
