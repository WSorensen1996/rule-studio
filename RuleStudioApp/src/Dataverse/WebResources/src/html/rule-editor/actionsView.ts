// Section 3, Actions: ordered THEN and ELSE lists. Each action type shows its own inputs.
import { TEXT_TARGET } from "./fixedValue";
import { clearMappings, mappingEditor } from "./mappingsView";
import { ActionType, Branch, emptyValue, newAction, SourceType, Stage } from "./model";
import { ALL_SOURCES, disableAll, sourceSelect, valueControl } from "./valueEditor";
import { rs_messagelevel as MessageLevel } from "../../../typings/XRM/OptionSets";
import { h } from "../../shared/dom";
import { findColumn, loadTable } from "../../shared/tableMetadata";
import { withHelp } from "../../shared/tooltip";

import type { EditorContext } from "./context";
import type { ValueTarget } from "./fixedValue";
import type { ActionNode } from "./model";

const TYPES: [number, string][] = [
  [ActionType.Setfield, "Set field"], [ActionType.Blocksave, "Block save"],
  [ActionType.Callfunction, "Call function"], [ActionType.Logmessage, "Log message"],
];
const LEVELS: [number, string][] = [[MessageLevel.Info, "Info"], [MessageLevel.Warning, "Warning"], [MessageLevel.Error, "Error"]];

function select(label: string, items: [number | string, string][], value: number | string | null, onPick: (value: string) => void): HTMLSelectElement {
  const element = h("select", { className: "select", attrs: { "aria-label": label } },
    ...items.map(([v, text]) => h("option", { attrs: { value: String(v) } }, text)));
  element.value = value == null ? "" : String(value);
  element.addEventListener("change", () => onPick(element.value));
  return element;
}

function field(label: string, control: HTMLElement): HTMLElement {
  return h("div", { className: "field" }, h("span", { className: "form-label" }, label), control);
}

function textInput(label: string, value: string, onInput: (value: string) => void): HTMLInputElement {
  const input = h("input", { className: "input", attrs: { type: "text", "aria-label": label } });
  input.value = value;
  input.addEventListener("change", () => onInput(input.value));
  return input;
}

function targetFieldSelect(action: ActionNode, context: EditorContext): HTMLSelectElement {
  const element = select("Field to set", [["", action.targetField || "— choose —"]], "", (value) => {
    action.targetField = value;
    context.changed(true);
  });
  const table = context.state.version.targetTable;
  if (!table) return element;
  void loadTable(table).then((info) => {
    const writable = info.columns.filter((c) => c.writable || c.logicalName === action.targetField);
    element.replaceChildren(h("option", { attrs: { value: "" } }, "— choose —"),
      ...writable.map((c) => h("option", { attrs: { value: c.logicalName } }, `${c.label} (${c.logicalName})`)));
    element.value = action.targetField;
  }, () => {});
  return element;
}

async function setFieldTarget(action: ActionNode, table: string): Promise<ValueTarget> {
  const column = await findColumn(table, action.targetField);
  return column ? { kind: column.kind, options: column.options, targets: column.targets } : TEXT_TARGET;
}

function functionInputs(action: ActionNode, context: EditorContext): HTMLElement {
  const { state, catalog, disabled } = context;
  const fn = catalog.functions.find((f) => f.id === action.value.customFunctionId);
  return mappingEditor(fn, action.mappings, { rootTable: state.version.targetTable, catalog, disabled, removed: state.removed, onChange: context.changed });
}

function setFieldBody(action: ActionNode, context: EditorContext): HTMLElement[] {
  const table = context.state.version.targetTable;
  const usedFunction = action.value.customFunctionId;
  const options = {
    value: action.value, rootTable: table, target: () => setFieldTarget(action, table), multiplicity: "one" as const,
    sources: ALL_SOURCES, catalog: context.catalog, disabled: context.disabled, label: "New value",
    onChange: (structural: boolean) => {
      if (action.value.customFunctionId !== usedFunction) clearMappings(action.mappings, context.state.removed);
      context.changed(structural);
    },
  };
  const body: HTMLElement[] = [h("div", { className: "action-line set-field-line" },
    field("Field to set", withHelp(targetFieldSelect(action, context), "targetField")), field("Value source", sourceSelect(options)), field("New value", valueControl(options)))];
  if (action.value.sourceType === SourceType.Function) body.push(functionInputs(action, context));
  return body;
}

function callFunctionBody(action: ActionNode, context: EditorContext): HTMLElement[] {
  const functions = context.catalog.functions.filter((f) => f.enabled || f.id === action.value.customFunctionId);
  const picker = select("Registered Custom Function", [["", functions.length ? "— choose —" : "No functions registered"], ...functions.map((f): [string, string] => [f.id, f.enabled ? f.name : `⚠ ${f.name} (inactive)`])],
    action.value.customFunctionId, (id) => {
      clearMappings(action.mappings, context.state.removed);
      action.value = { ...emptyValue(), sourceType: SourceType.Function, customFunctionId: id || null };
      context.changed(true);
    });
  const fn = context.catalog.functions.find((f) => f.id === action.value.customFunctionId);
  return [
    h("div", { className: "action-line two" }, field("Registered Custom Function", withHelp(picker, "customFunction")),
      field("Custom API", h("div", { className: "input readonly mono" }, fn?.apiName ?? "—"))),
    functionInputs(action, context),
  ];
}

function messageBody(action: ActionNode, context: EditorContext): HTMLElement[] {
  const set = (value: string): void => {
    action.message = value;
    context.changed(false);
  };
  if (action.actionType === ActionType.Blocksave) {
    const body: HTMLElement[] = [h("div", { className: "action-line one" }, field("Error shown to the user", withHelp(textInput("Error shown to the user", action.message, set), "blockMessage")))];
    if (context.state.version.stage === Stage.Aftersave) body.push(h("p", { className: "notice bad t11" }, "Block save only works Before save. Change the stage or the action type."));
    return body;
  }
  const level = select("Message level", LEVELS, action.messageLevel ?? MessageLevel.Info, (value) => {
    action.messageLevel = Number(value);
    context.changed(false);
  });
  return [h("div", { className: "action-line two" }, field("Message", textInput("Log message", action.message, set)), field("Level", withHelp(level, "messageLevel")))];
}

function body(action: ActionNode, context: EditorContext): HTMLElement[] {
  switch (action.actionType) {
    case ActionType.Callfunction: return callFunctionBody(action, context);
    case ActionType.Setfield: return setFieldBody(action, context);
    default: return messageBody(action, context);
  }
}

function controls(action: ActionNode, list: ActionNode[], context: EditorContext): HTMLElement {
  const index = list.indexOf(action);
  const button = (text: string, label: string, enabled: boolean, onClick: () => void): HTMLButtonElement => {
    const element = h("button", { className: "btn mini square", attrs: { type: "button", title: label, "aria-label": label } }, text);
    element.disabled = !enabled;
    element.addEventListener("click", () => {
      onClick();
      context.changed(true);
    });
    return element;
  };
  const swap = (other: ActionNode | undefined): void => {
    const all = context.state.version.actions;
    if (!other) return;
    const a = all.indexOf(action);
    const b = all.indexOf(other);
    [all[a], all[b]] = [other, action];
  };
  return h("div", { className: "action-controls" },
    button("↑", "Move up", index > 0, () => swap(list[index - 1])),
    button("↓", "Move down", index < list.length - 1, () => swap(list[index + 1])),
    button("×", "Remove action", true, () => {
      clearMappings(action.mappings, context.state.removed);
      if (!action.isNew) context.state.removed.actions.push(action.id);
      const all = context.state.version.actions;
      all.splice(all.indexOf(action), 1);
    }));
}

function actionRow(action: ActionNode, list: ActionNode[], context: EditorContext): HTMLElement {
  const type = select("Action type", TYPES, action.actionType, (value) => {
    clearMappings(action.mappings, context.state.removed);
    Object.assign(action, { actionType: Number(value), targetField: "", value: emptyValue(), message: "", messageLevel: null });
    context.changed(true);
  });
  const row = h("div", { className: "action-row", attrs: { "data-id": action.id } },
    h("div", { className: "action-index" }, String(list.indexOf(action) + 1)),
    h("div", {}, h("div", { className: "field action-type" }, h("span", { className: "form-label" }, "Action type"), withHelp(type, "actionType")), ...body(action, context)),
    controls(action, list, context));
  disableAll(row, context.disabled);
  return row;
}

function branchBlock(branch: number, context: EditorContext): HTMLElement {
  const list = context.state.version.actions.filter((a) => a.branch === branch);
  const name = branch === Branch.Then ? "then" : "else";
  const add = h("button", { className: "btn mini", attrs: { type: "button" } }, `+ Add ${name} action`);
  add.disabled = context.disabled;
  add.addEventListener("click", () => {
    context.state.version.actions.push(newAction(branch));
    context.changed(true);
  });
  const rows = list.length
    ? list.map((action) => actionRow(action, list, context))
    : [h("div", { className: "editor-placeholder" }, branch === Branch.Then
      ? "No actions yet: add what happens when the conditions match."
      : "No else actions: nothing happens when the conditions do not match.")];
  return h("div", { className: "branch" },
    h("div", { className: "branch-head" }, h("h3", { className: "action-branch" }, `${name.toUpperCase()} ACTIONS`), add), ...rows);
}

export function renderActions(container: HTMLElement, context: EditorContext): void {
  container.replaceChildren(branchBlock(Branch.Then, context), branchBlock(Branch.Else, context));
}
