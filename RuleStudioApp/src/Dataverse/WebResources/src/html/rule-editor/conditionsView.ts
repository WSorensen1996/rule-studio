// Section 2, Conditions: the AND/OR group tree with field path → operator → value rows.
import { TEXT_TARGET } from "./fixedValue";
import { clearMappings, mappingEditor } from "./mappingsView";
import { Aggregate, collectRemoved, Logical, newCondition, newGroup, Operator, RuleEvent, SourceType } from "./model";
import { pathPicker } from "./pathPicker";
import { columnOf, hasChildHop } from "./paths";
import { ALL_SOURCES, disableAll, sourceSelect, valueControl } from "./valueEditor";
import { h } from "../../shared/dom";
import { withHelp } from "../../shared/tooltip";

import type { EditorContext } from "./context";
import type { Multiplicity, ValueTarget } from "./fixedValue";
import type { ConditionNode, GroupNode } from "./model";

const OPERATORS: [number, string][] = [
  [Operator.Equals, "Equals"], [Operator.Notequals, "Not equals"], [Operator.Greaterthan, "Greater than"],
  [Operator.Greaterorequal, "Greater or equal"], [Operator.Lessthan, "Less than"], [Operator.Lessorequal, "Less or equal"],
  [Operator.Contains, "Contains"], [Operator.Startswith, "Starts with"], [Operator.Isempty, "Is empty"],
  [Operator.Isnotempty, "Is not empty"], [Operator.Changed, "Changed"], [Operator.Inlist, "In list"], [Operator.Between, "Between"],
];
const AGGREGATES: [number, string][] = [
  [Aggregate.None, "None"], [Aggregate.Any, "Any"], [Aggregate.All, "All"], [Aggregate.Noneof, "None-of"],
  [Aggregate.Count, "Count"], [Aggregate.Sum, "Sum"], [Aggregate.Min, "Min"], [Aggregate.Max, "Max"], [Aggregate.Average, "Average"],
];
const NO_VALUE = new Set<number>([Operator.Isempty, Operator.Isnotempty, Operator.Changed]);
const NUMERIC_AGGREGATES = new Set<number>([Aggregate.Count, Aggregate.Sum, Aggregate.Min, Aggregate.Max, Aggregate.Average]);

function choiceSelect(label: string, items: [number, string][], value: number, onPick: (value: number) => void): HTMLSelectElement {
  const select = h("select", { className: "select", attrs: { "aria-label": label } },
    ...items.map(([v, text]) => h("option", { attrs: { value: String(v) } }, text)));
  select.value = String(value);
  select.addEventListener("change", () => onPick(Number(select.value)));
  return select;
}

function cell(label: string, control: HTMLElement): HTMLElement {
  return h("div", {}, h("span", { className: "micro-label" }, label), control);
}

function multiplicity(operator: number): Multiplicity {
  if (operator === Operator.Inlist) return "list";
  return operator === Operator.Between ? "range" : "one";
}

async function conditionTarget(condition: ConditionNode): Promise<ValueTarget> {
  if (NUMERIC_AGGREGATES.has(condition.aggregate)) return { ...TEXT_TARGET, kind: "number" };
  const column = await columnOf(condition.fieldPath);
  return column ? { kind: column.kind, options: column.options, targets: column.targets } : TEXT_TARGET;
}

function detail(condition: ConditionNode, context: EditorContext): HTMLElement {
  const notes: string[] = [];
  if (hasChildHop(condition.fieldPath)) {
    notes.push(condition.aggregate === Aggregate.None
      ? "⚠ Child-record path: choose Any, All or None-of, or an aggregate such as Count or Sum."
      : "↳ Child-record path, evaluated with the chosen child operator.");
  }
  if (condition.operator === Operator.Changed && context.state.version.event !== RuleEvent.Update) {
    notes.push("⚠ Changed only works on an Update trigger.");
  }
  return h("div", { className: "cond-detail" }, notes.join(" "));
}

function conditionRow(condition: ConditionNode, parent: GroupNode, context: EditorContext): HTMLElement {
  const { state, catalog, disabled } = context;
  const rootTable = state.version.targetTable;
  const childPath = hasChildHop(condition.fieldPath);
  const set = (change: Partial<ConditionNode>, structural = true): void => {
    Object.assign(condition, change);
    context.changed(structural);
  };
  const path = pathPicker({
    rootTable, path: condition.fieldPath, allowChild: true, disabled, label: "Field path",
    onChange: (fieldPath) => set({ fieldPath, aggregate: hasChildHop(fieldPath) ? condition.aggregate : Aggregate.None }),
  });
  const aggregate = withHelp(choiceSelect("Child operator", AGGREGATES, condition.aggregate, (value) => set({ aggregate: value })), "aggregate");
  aggregate.disabled = disabled || !childPath;
  const operator = withHelp(choiceSelect("Comparison", OPERATORS, condition.operator, (value) => set({ operator: value })), "operator");
  const usedFunction = condition.value.customFunctionId;
  const valueOptions = {
    value: condition.value, rootTable, target: () => conditionTarget(condition), multiplicity: multiplicity(condition.operator),
    sources: ALL_SOURCES, catalog, disabled, label: "Condition",
    onChange: (structural: boolean) => {
      if (condition.value.customFunctionId !== usedFunction) clearMappings(condition.mappings, state.removed);
      context.changed(structural);
    },
  };
  const hasValue = !NO_VALUE.has(condition.operator);
  const remove = h("button", { className: "btn mini square", attrs: { type: "button", "aria-label": "Remove condition" } }, "×");
  remove.addEventListener("click", () => {
    collectRemoved(condition, state.removed);
    parent.children.splice(parent.children.indexOf(condition), 1);
    context.changed(true);
  });
  const grid = h("div", { className: "cond-grid" },
    cell("FIELD PATH", path), cell("CHILD OPERATOR", aggregate), cell("COMPARISON", operator),
    cell("VALUE SOURCE", hasValue ? sourceSelect(valueOptions) : h("span", { className: "muted t11" }, "—")),
    cell("VALUE", hasValue ? valueControl(valueOptions) : h("span", { className: "muted t11" }, "No value needed")),
    remove);
  disableAll(grid, disabled);
  const row = h("div", { className: "cond-row", attrs: { "data-id": condition.id } }, grid, detail(condition, context));
  if (hasValue && condition.value.sourceType === SourceType.Function) {
    const fn = catalog.functions.find((f) => f.id === condition.value.customFunctionId);
    row.append(mappingEditor(fn, condition.mappings, { rootTable, catalog, disabled, removed: state.removed, onChange: context.changed }));
  }
  return row;
}

function groupHead(group: GroupNode, parent: GroupNode | null, context: EditorContext): HTMLElement {
  const operator = withHelp(choiceSelect(parent ? "Nested group operator" : "Root group operator", [[Logical.AND, "AND"], [Logical.OR, "OR"]], group.operator, (value) => {
    group.operator = value;
    context.changed(true);
  }), "groupOperator");
  operator.classList.add("group-operator");
  const button = (text: string, label: string, onClick: () => void): HTMLButtonElement => {
    const element = h("button", { className: `btn mini${text === "×" ? " square" : ""}`, attrs: { type: "button", "aria-label": label } }, text);
    element.addEventListener("click", () => {
      onClick();
      context.changed(true);
    });
    return element;
  };
  const buttons = [
    button("+ Condition", "Add condition", () => group.children.push(newCondition())),
    button("+ AND/OR group", "Add nested group", () => group.children.push(newGroup(group.operator === Logical.AND ? Logical.OR : Logical.AND))),
  ];
  if (parent) {
    buttons.push(button("×", "Remove group", () => {
      collectRemoved(group, context.state.removed);
      parent.children.splice(parent.children.indexOf(group), 1);
    }));
  }
  const head = h("div", { className: "group-head" },
    h("div", { className: "group-title" }, h("span", { className: "group-label" }, parent ? "Nested group" : "Root logic"), operator,
      h("span", { className: "muted t11" }, group.operator === Logical.OR ? "At least one condition must match" : "All conditions must match")),
    h("div", { className: "btn-group" }, ...buttons));
  disableAll(head, context.disabled);
  return head;
}

function groupBox(group: GroupNode, parent: GroupNode | null, context: EditorContext): HTMLElement {
  const children = group.children.map((child) => (child.kind === "group"
    ? groupBox(child, group, context)
    : conditionRow(child, group, context)));
  if (children.length === 0) {
    children.push(h("div", { className: "editor-placeholder" }, parent ? "Empty group: add a condition or remove the group." : "No conditions: the actions run on every triggering event."));
  }
  return h("div", { className: "group-box", attrs: { "data-id": group.id } }, groupHead(group, parent, context), h("div", { className: "group-children" }, ...children));
}

export function renderConditions(container: HTMLElement, context: EditorContext): void {
  container.replaceChildren(groupBox(context.state.version.root, null, context));
}
