import { renderCriteria } from "./criteriaEditor";
import { DraftError, newCriterion, toDocument, toDraft } from "./criteriaModel";
import { applyRole } from "../../shared/auth";
import { goBackAfterSave, setLeaveGuard } from "../../shared/back";
import { byId, fillSelect } from "../../shared/dom";
import { goBack, pageParams } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { loadTable, loadTableList } from "../../shared/tableMetadata";
import { initWebApi } from "../../shared/xrm";
import { confirmDeleteLookup, deleteLookup, loadLookup, saveLookup } from "../lookup-definitions/lookupData";
import { renderMap } from "../lookup-definitions/mapView";
import { MULTI_MATCH_OPTIONS, MultiMatch, NO_MATCH_OPTIONS, NoMatch } from "../lookup-definitions/policies";
import { decodeFixed, fixedInput, TEXT_TARGET } from "../rule-editor/fixedValue";
import { disableAll } from "../rule-editor/valueEditor";

import type { CriterionDraft, ValueDraft } from "./criteriaModel";
import type { CriterionValueSource } from "../../shared/formats";
import type { ColumnInfo, TableName } from "../../shared/tableMetadata";
import type { LookupDefinition } from "../lookup-definitions/lookupData";

applyStandard();

const nameInput = byId<HTMLInputElement>("lookupName");
const descriptionInput = byId<HTMLInputElement>("lookupDescription");
const recordTable = byId<HTMLSelectElement>("recordTable");
const lookupTable = byId<HTMLSelectElement>("lookupTable");
const returnColumn = byId<HTMLSelectElement>("lookupReturn");
const sortColumn = byId<HTMLSelectElement>("lookupSort");
const noMatch = byId<HTMLSelectElement>("lookupNoMatch");
const multiMatch = byId<HTMLSelectElement>("lookupMulti");
const statusLine = (): HTMLElement => byId("status");

let lookupId: string | null = null;
let drafts: CriterionDraft[] = [];
let columns: ColumnInfo[] = [];
let defaultRaw = "";
let author = false;
// The form as last loaded or saved; Back asks first when the form differs from it.
let saved = "";

function tableOptions(tables: TableName[], current: string): { value: string; label: string }[] {
  const options = [{ value: "", label: "— choose a table —" }, ...tables.map((t) => ({ value: t.logicalName, label: `${t.label} (${t.logicalName})` }))];
  if (current && !tables.some((t) => t.logicalName === current)) options.push({ value: current, label: `⚠ ${current} (not found)` });
  return options;
}

function columnOptions(current: string, empty: string): { value: string; label: string }[] {
  const options = [{ value: "", label: empty }, ...columns.map((c) => ({ value: c.logicalName, label: `${c.label} (${c.logicalName})` }))];
  if (current && !columns.some((c) => c.logicalName === current)) options.push({ value: current, label: `⚠ ${current} (not found)` });
  return options;
}

function redrawCriteria(): void {
  renderCriteria(byId("criteria"), { drafts, recordTable: recordTable.value, columns, disabled: !author, redraw: redrawCriteria });
  redrawMap();
}

/** The record-side value of a draft, or null while it is incomplete. */
function draftSource(value: ValueDraft): CriterionValueSource | null {
  if (value.source === "fieldPath") return value.path ? { type: "fieldPath", path: value.path } : null;
  if (value.source === "expression") return value.expression.trim() ? { type: "expression", expression: value.expression.trim() } : null;
  const fixed = decodeFixed(value.raw)[0];
  return fixed === undefined ? null : { type: "fixed", value: fixed };
}

function draftValues(draft: CriterionDraft): ValueDraft[] {
  if (draft.operator === "in") return draft.values;
  return draft.operator === "isEmpty" ? [] : [draft.value];
}

function redrawMap(): void {
  const criteria = drafts.map((d) => ({
    operator: d.operator,
    columns: d.operator === "range" ? [d.minColumn, d.maxColumn] : [d.column],
    values: draftValues(d).map(draftSource).filter((s): s is CriterionValueSource => s !== null),
    wildcard: d.operator !== "isEmpty" && d.emptyMatchesAll,
  }));
  byId("fieldMap").replaceChildren(renderMap({ recordTable: recordTable.value, lookupTable: lookupTable.value, returnColumn: returnColumn.value, criteria }));
}

function renderDefault(): void {
  const field = byId("defaultField");
  field.hidden = Number(noMatch.value) !== NoMatch.Usedefault;
  const info = columns.find((c) => c.logicalName === returnColumn.value);
  const target = info ? { kind: info.kind, options: info.options, targets: info.targets } : TEXT_TARGET;
  const holder = byId("defaultHolder");
  holder.replaceChildren(fixedInput(target, defaultRaw, "one", (raw) => {
    defaultRaw = raw;
  }));
  disableAll(holder, !author);
}

function fillColumnSelects(): void {
  fillSelect(returnColumn, columnOptions(returnColumn.value, columns.length ? "— choose a column —" : "Choose the table with the answers first"));
  fillSelect(sortColumn, columnOptions(sortColumn.value, "No sort column"));
}

async function loadColumns(): Promise<void> {
  columns = [];
  if (lookupTable.value) {
    try {
      columns = (await loadTable(lookupTable.value)).columns;
    } catch (error) {
      setStatus(statusLine(), describeError(`read the columns of ${lookupTable.value}`, error), "error");
    }
  }
  fillColumnSelects();
  renderDefault();
  redrawCriteria();
}

/** Puts the stored values into select elements that need an option for them first. */
function setSelect(select: HTMLSelectElement, value: string): void {
  if (value && !Array.from(select.options).some((o) => o.value === value)) {
    fillSelect(select, [...Array.from(select.options).map((o) => ({ value: o.value, label: o.text })), { value, label: value }]);
  }
  select.value = value;
}

function fillForm(lookup: LookupDefinition): void {
  nameInput.value = lookup.name;
  descriptionInput.value = lookup.description;
  setSelect(recordTable, lookup.criteria?.recordTable ?? "");
  setSelect(lookupTable, lookup.lookupTable);
  setSelect(returnColumn, lookup.returnColumn);
  setSelect(sortColumn, lookup.sortColumn);
  if (lookup.noMatchPolicy !== null) noMatch.value = String(lookup.noMatchPolicy);
  if (lookup.multiMatchPolicy !== null) multiMatch.value = String(lookup.multiMatchPolicy);
  defaultRaw = lookup.defaultValue;
  drafts = lookup.criteria?.criteria.map(toDraft) ?? [];
  showTitle(lookup.name);
  byId("intro").textContent = "Rules that use this lookup pick up a change when they are next published.";
  if (lookup.criteriaProblem) setStatus(statusLine(), `${lookup.criteriaProblem} Saving replaces the stored match criteria.`, "error");
}

function showTitle(title: string): void {
  byId("pageTitle").textContent = title || "Lookup definition";
  document.title = `${title || "Lookup definition"} · Rule Studio`;
}

const snapshot = (): string => JSON.stringify([
  nameInput.value, descriptionInput.value, recordTable.value, lookupTable.value, returnColumn.value,
  sortColumn.value, noMatch.value, multiMatch.value, defaultRaw, drafts,
]);

function required(): string | null {
  if (!nameInput.value.trim()) return "Enter a lookup name.";
  if (!recordTable.value) return "Choose the record being saved.";
  if (!lookupTable.value) return "Choose the table with the answers.";
  if (!returnColumn.value) return "Choose the return column.";
  if (Number(multiMatch.value) === MultiMatch.Firstbysort && !sortColumn.value) return "Choose a sort column for \"First by sort\".";
  return null;
}

/** Saves the definition; resolves true when it worked. */
async function save(): Promise<boolean> {
  const problem = required();
  if (problem) {
    setStatus(statusLine(), problem, "error");
    return false;
  }
  const buttons = [byId<HTMLButtonElement>("save"), byId<HTMLButtonElement>("cmdSaveClose")];
  try {
    const document = toDocument(recordTable.value, drafts);
    for (const button of buttons) button.disabled = true;
    setStatus(statusLine(), "Saving…");
    const created = !lookupId;
    lookupId = await saveLookup(lookupId, {
      name: nameInput.value, description: descriptionInput.value, lookupTable: lookupTable.value,
      returnColumn: returnColumn.value, sortColumn: sortColumn.value,
      defaultValue: Number(noMatch.value) === NoMatch.Usedefault ? defaultRaw : "",
      matchCriteriaText: JSON.stringify(document),
      noMatchPolicy: Number(noMatch.value), multiMatchPolicy: Number(multiMatch.value),
    });
    toast(created ? "Lookup definition created." : "Lookup definition saved.");
    saved = snapshot();
    showTitle(nameInput.value.trim());
    byId("cmdDelete").hidden = false;
    setStatus(statusLine(), "");
    return true;
  } catch (error) {
    const message = error instanceof DraftError ? error.message : describeError("save the lookup definition", error);
    setStatus(statusLine(), message, "error");
    return false;
  } finally {
    for (const button of buttons) button.disabled = !author;
  }
}

/** Deletes the saved definition and leaves the page; a lookup in use cannot be deleted. */
async function remove(): Promise<void> {
  if (!lookupId) return;
  const button = byId<HTMLButtonElement>("cmdDelete");
  try {
    if (!(await confirmDeleteLookup(nameInput.value.trim() || "this lookup"))) return;
    button.disabled = true;
    await deleteLookup(lookupId);
    toast("Lookup definition deleted.");
    goBack();
  } catch (error) {
    setStatus(statusLine(), describeError("delete the lookup definition (it may be in use)", error), "error");
  } finally {
    button.disabled = !author;
  }
}

function bind(): void {
  lookupTable.addEventListener("change", () => void loadColumns());
  recordTable.addEventListener("change", redrawCriteria);
  returnColumn.addEventListener("change", () => {
    defaultRaw = "";
    renderDefault();
    redrawMap();
  });
  // Fixed values and expressions change without a redraw of the grid.
  byId("criteria").addEventListener("change", redrawMap);
  noMatch.addEventListener("change", renderDefault);
  byId("addCriterion").addEventListener("click", () => {
    drafts.push(newCriterion());
    redrawCriteria();
  });
  byId("save").addEventListener("click", () => void save());
  byId("cmdSaveClose").addEventListener("click", () => void goBackAfterSave(save));
  byId("cmdDelete").addEventListener("click", () => void remove());
  setLeaveGuard({ dirty: () => author && snapshot() !== saved, save });
}

async function load(): Promise<void> {
  const tables = await loadTableList().catch((error: unknown) => {
    setStatus(statusLine(), describeError("load the table list", error), "error");
    return [] as TableName[];
  });
  fillSelect(recordTable, tableOptions(tables, ""));
  fillSelect(lookupTable, tableOptions(tables, ""));
  if (lookupId) {
    const lookup = await loadLookup(lookupId);
    fillSelect(recordTable, tableOptions(tables, lookup.criteria?.recordTable ?? ""));
    fillSelect(lookupTable, tableOptions(tables, lookup.lookupTable));
    fillForm(lookup);
    byId("cmdDelete").hidden = false;
  } else {
    // A new definition starts with one empty criterion; remove it to match every row.
    drafts = [newCriterion()];
  }
  await loadColumns();
}

function start(): void {
  initWebApi();
  author = applyRole();
  lookupId = pageParams().lookupId ?? null;
  fillSelect(noMatch, NO_MATCH_OPTIONS.map((o) => ({ value: String(o.value), label: o.label })));
  fillSelect(multiMatch, MULTI_MATCH_OPTIONS.map((o) => ({ value: String(o.value), label: o.label })));
  noMatch.value = String(NoMatch.Block);
  multiMatch.value = String(MultiMatch.Mostspecific);
  bind();
  void load()
    .then(() => {
      saved = snapshot();
      if (!byId("status").classList.contains("error")) setStatus(statusLine(), "");
    })
    .catch((error: unknown) => setStatus(statusLine(), describeError("load the lookup definition", error), "error"));
}

start();
