// "In plain words": the rule read back as one sentence (WHEN … IF … THEN … ELSE …), rebuilt from
// the editor model on every change. Column and table names come from the cached metadata.
import { ActionType, Aggregate, Branch, Logical, Operator, RuleEvent, SourceType, Stage } from "./model";
import { columnOf, endTable } from "./paths";
import { rs_messagelevel as MessageLevel } from "../../../typings/XRM/OptionSets";
import { h } from "../../shared/dom";
import { findColumn, tableLabel } from "../../shared/tableMetadata";

import type { Catalog } from "./catalog";
import type { ActionNode, ConditionNode, EditorState, GroupNode, ValueModel } from "./model";
import type { FieldPath } from "../../shared/formats";
import type { ColumnInfo } from "../../shared/tableMetadata";

type Part = Node | string;

const OPERATOR_TEXT: Record<number, string> = {
  [Operator.Equals]: "is", [Operator.Notequals]: "is not", [Operator.Greaterthan]: "is more than",
  [Operator.Greaterorequal]: "is at least", [Operator.Lessthan]: "is less than", [Operator.Lessorequal]: "is at most",
  [Operator.Contains]: "contains", [Operator.Startswith]: "starts with", [Operator.Isempty]: "is empty",
  [Operator.Isnotempty]: "is filled in", [Operator.Changed]: "has changed", [Operator.Inlist]: "is one of", [Operator.Between]: "is between",
};
const NO_VALUE = new Set<number>([Operator.Isempty, Operator.Isnotempty, Operator.Changed]);
const NUMERIC = new Set<number>([Aggregate.Count, Aggregate.Sum, Aggregate.Min, Aggregate.Max, Aggregate.Average]);
const EVENT_TEXT: Record<number, string> = { [RuleEvent.Create]: "created", [RuleEvent.Update]: "updated", [RuleEvent.Delete]: "deleted" };
const LEVEL_TEXT: Record<number, string> = { [MessageLevel.Info]: "an info message", [MessageLevel.Warning]: "a warning", [MessageLevel.Error]: "an error" };

const v = (text: string): HTMLElement => h("b", { className: "rs-v" }, text);
const phase = (text: string, muted = false): HTMLElement => h("span", { className: `phase${muted ? " muted" : ""}` }, text);

let renderToken = 0;

/** Rebuilds the sentence; a slower earlier build never overwrites a newer one. */
export function renderReadout(container: HTMLElement, state: EditorState, catalog: Catalog): void {
  const token = ++renderToken;
  void sentence(state, catalog)
    .then((parts) => {
      if (token === renderToken) container.replaceChildren(...parts);
    })
    .catch(() => {
      if (token === renderToken) container.replaceChildren(h("span", { className: "muted" }, "The rule can't be read back right now."));
    });
}

async function sentence(state: EditorState, catalog: Catalog): Promise<Part[]> {
  const { version } = state;
  if (!version.targetTable) return [h("span", { className: "muted" }, "Choose a target table in section 1 to see the rule in plain words.")];
  const table = await tableLabel(version.targetTable);
  const parts: Part[] = [phase("WHEN"), " a ", v(table), " is ", v(EVENT_TEXT[version.event] ?? "saved"), ", ",
    version.stage === Stage.Aftersave ? "after it's saved" : "before it's saved"];
  const filtered = version.event === RuleEvent.Update ? version.filteredColumns.split(/[,; ]+/).filter(Boolean) : [];
  if (filtered.length > 0) {
    const names = await Promise.all(filtered.map(async (c) => (await findColumn(version.targetTable, c.toLowerCase()))?.label ?? c));
    parts.push(" and ", v(names.join(" or ")), filtered.length > 1 ? " are part of the save" : " is part of the save");
  }
  parts.push(", ", phase("IF"), " ", ...(await group(version.root, true, catalog)), ", ");
  const then = version.actions.filter((a) => a.branch !== Branch.Else);
  const otherwise = version.actions.filter((a) => a.branch === Branch.Else);
  // No full stops after the branches: a quoted message usually ends with its own.
  parts.push(phase("THEN"), " ", ...(await actions(then, version.targetTable, catalog)), " ");
  parts.push(phase("ELSE", true), " ", ...(otherwise.length ? await actions(otherwise, version.targetTable, catalog) : ["do nothing"]));
  return parts;
}

async function group(node: GroupNode, top: boolean, catalog: Catalog): Promise<Part[]> {
  const children: Part[][] = [];
  for (const child of node.children) children.push(child.kind === "group" ? await group(child, false, catalog) : await condition(child, catalog));
  const filled = children.filter((c) => c.length > 0);
  if (filled.length === 0) return top ? ["always (there are no conditions)"] : [];
  const word = node.operator === Logical.OR ? "or" : "and";
  const joined = filled.flatMap((c, i) => (i === 0 ? c : [" ", h("span", { className: "rs-join" }, word), " ", ...c]));
  return !top && filled.length > 1 ? ["(", ...joined, ")"] : joined;
}

/** Labels along a path: the start table, each parent lookup or child table, and the column. */
async function pathLabels(path: FieldPath): Promise<string[]> {
  const labels = [await tableLabel(path.table)];
  let table = path.table;
  for (const hop of path.hops) {
    labels.push(hop.kind === "parent" ? ((await findColumn(table, hop.attribute))?.label ?? hop.attribute) : await tableLabel(hop.table));
    table = hop.table;
  }
  labels.push((await findColumn(endTable(path), path.column))?.label ?? path.column);
  return labels;
}

/** "the Account's Customer segment" for a path on the rule's own table (the start table is left out). */
async function ownPath(path: FieldPath): Promise<string> {
  const labels = (await pathLabels(path)).slice(1);
  return `the ${labels.join("'s ")}`;
}

type ChildPhrase = (col: string, child: string, op: string) => Part[];
const CHILD_PHRASE: Record<number, ChildPhrase> = {
  [Aggregate.Count]: (_, child, op) => ["the number of ", v(`${child} records`), ` ${op}`],
  [Aggregate.Any]: (col, child, op) => ["at least one ", v(child), ` has ${col} that ${op}`],
  [Aggregate.All]: (col, child, op) => ["every ", v(child), ` has ${col} that ${op}`],
  [Aggregate.Noneof]: (col, child, op) => ["no ", v(child), ` has ${col} that ${op}`],
  [Aggregate.Sum]: (col, child, op) => [`the sum of ${col} on `, v(`${child} records`), ` ${op}`],
  [Aggregate.Min]: (col, child, op) => [`the lowest ${col} on `, v(`${child} records`), ` ${op}`],
  [Aggregate.Max]: (col, child, op) => [`the highest ${col} on `, v(`${child} records`), ` ${op}`],
  [Aggregate.Average]: (col, child, op) => [`the average ${col} on `, v(`${child} records`), ` ${op}`],
};
const NO_AGGREGATE: ChildPhrase = (col, child, op) => [`${col} on `, v(`${child} records`), ` ${op}`, h("span", { className: "muted" }, " (choose Any, All, None-of or a count)")];

async function childSubject(path: FieldPath, aggregate: number, op: string): Promise<Part[]> {
  const labels = await pathLabels(path);
  const childHop = path.hops.filter((hop) => hop.kind === "child").at(-1);
  const childTable = await tableLabel(childHop ? childHop.table : endTable(path));
  return (CHILD_PHRASE[aggregate] ?? NO_AGGREGATE)(labels[labels.length - 1], childTable, op);
}

async function condition(node: ConditionNode, catalog: Catalog): Promise<Part[]> {
  const path = node.fieldPath;
  if (!path) return [h("span", { className: "muted" }, "(a condition without a field)")];
  const op = OPERATOR_TEXT[node.operator] ?? "is";
  const column = NUMERIC.has(node.aggregate) ? undefined : await columnOf(path);
  const value: Part[] = NO_VALUE.has(node.operator) ? [] : [" ", ...(await valueParts(node.value, column, node.operator, catalog))];
  const subject = path.hops.some((hop) => hop.kind === "child") ? await childSubject(path, node.aggregate, op) : [await ownPath(path), ` ${op}`];
  // All over no child records is true; say so after the value.
  const none = node.aggregate === Aggregate.All ? [h("span", { className: "muted" }, " (also true when there are none)")] : [];
  return [...subject, ...value, ...none];
}

function referenceText(raw: object): string {
  const ref = raw as { name?: string; id?: string };
  return ref.name ?? ref.id ?? JSON.stringify(raw);
}

function choiceText(raw: string | number, column: ColumnInfo): string {
  const option = column.options.find((o) => o.value === Number(raw));
  return option ? option.label : String(raw);
}

function scalarText(raw: string | number, column: ColumnInfo | undefined): string {
  if (column?.kind === "choice") return choiceText(raw, column);
  if (column?.kind === "boolean") return String(raw) === "1" || raw === "true" ? "Yes" : "No";
  return typeof raw === "string" && /^\d{4}-\d{2}-\d{2}T/.test(raw) ? raw.slice(0, 10) : String(raw);
}

function fixedText(raw: unknown, column: ColumnInfo | undefined): string {
  if (raw === null || raw === undefined || raw === "") return "empty";
  if (typeof raw === "boolean") return raw ? "Yes" : "No";
  if (typeof raw === "object") return referenceText(raw);
  return typeof raw === "string" || typeof raw === "number" ? scalarText(raw, column) : JSON.stringify(raw);
}

function fixedParts(value: ValueModel, column: ColumnInfo | undefined, operator: number | null): Part[] {
  let raw: unknown = value.value;
  try {
    raw = JSON.parse(value.value);
  } catch {
    // Not JSON: a plain text value.
  }
  if (!Array.isArray(raw)) return [v(fixedText(raw, column))];
  return [v(raw.map((item) => fixedText(item, column)).join(operator === Operator.Between ? " and " : ", "))];
}

const named = (list: { id: string; name: string }[], id: string | null): string => list.find((x) => x.id === id)?.name ?? "(not chosen)";

async function valueParts(value: ValueModel, column: ColumnInfo | undefined, operator: number | null, catalog: Catalog): Promise<Part[]> {
  switch (value.sourceType) {
    case SourceType.Fieldpath: return value.valuePath ? [v(await ownPath(value.valuePath))] : [h("span", { className: "muted" }, "(no field chosen)")];
    case SourceType.Expression: return ["the formula ", h("code", {}, value.value || "…")];
    case SourceType.Lookup: return ["the lookup ", v(named(catalog.lookups, value.lookupDefinitionId))];
    case SourceType.Function: return ["the result of ", v(`${named(catalog.functions, value.customFunctionId)}()`)];
    default: return fixedParts(value, column, operator);
  }
}

async function actions(list: ActionNode[], table: string, catalog: Catalog): Promise<Part[]> {
  if (list.length === 0) return ["do nothing"];
  const parts: Part[][] = [];
  for (const action of list) parts.push(await actionParts(action, table, catalog));
  return parts.flatMap((p, i) => (i === 0 ? p : [", then ", ...p]));
}

const quoted = (message: string): HTMLElement => v(`“${message || "…"}”`);

async function setFieldParts(action: ActionNode, table: string, catalog: Catalog): Promise<Part[]> {
  const column = action.targetField ? await findColumn(table, action.targetField) : undefined;
  const name = column ? column.label : action.targetField || "(no field)";
  return ["set ", v(name), " to ", ...(await valueParts(action.value, column, null, catalog))];
}

async function actionParts(action: ActionNode, table: string, catalog: Catalog): Promise<Part[]> {
  switch (action.actionType) {
    case ActionType.Setfield: return setFieldParts(action, table, catalog);
    case ActionType.Blocksave: return ["stop the save and show ", quoted(action.message)];
    case ActionType.Logmessage: return [`write ${LEVEL_TEXT[action.messageLevel ?? MessageLevel.Info] ?? "a message"} to the execution log: `, quoted(action.message)];
    case ActionType.Callfunction: return ["call ", v(named(catalog.functions, action.value.customFunctionId))];
    default: return [];
  }
}
