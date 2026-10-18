import { matches, ruleRow, ruleTables, type Filters } from "./libraryView";
import { applyRole } from "../../shared/auth";
import { loadRules } from "../../shared/data/rules";
import { byId, fillSelect } from "../../shared/dom";
import { tableDisplayNames } from "../../shared/metadata";
import { openDialog, openRecord, pageParams } from "../../shared/nav";
import { describeError, setStatus } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { RuleSummary } from "../../shared/data/rules";

applyStandard();

const search = byId<HTMLInputElement>("librarySearch");
const table = byId<HTMLSelectElement>("libraryTable");
const status = byId<HTMLSelectElement>("libraryStatus");
const selectAll = byId<HTMLInputElement>("selectAll");

let rules: RuleSummary[] = [];
let tableNames = new Map<string, string>();
const selected = new Set<string>();
let author = false;

function filters(): Filters {
  return { search: search.value, table: table.value, status: status.value };
}

function updateSelection(): void {
  const count = selected.size;
  const button = byId<HTMLButtonElement>("publishSelected");
  button.textContent = count ? `Publish selected (${count})` : "Publish selected";
  button.disabled = !author || count === 0;
  if (author) button.title = count === 0 ? "Select one or more rules with a draft first." : "";
}

function render(): void {
  const shown = rules.filter((rule) => matches(rule, filters()));
  const handlers = { author, selected, onOpen: (rule: RuleSummary) => void openEditor({ ruleId: rule.id }), onSelect: updateSelection };
  byId("ruleRows").replaceChildren(...shown.map((rule) => ruleRow(rule, tableNames, handlers)));
  byId("ruleCount").textContent = `${shown.length} of ${rules.length} rules`;
  const empty = byId("emptyState");
  empty.hidden = shown.length > 0;
  empty.textContent = rules.length === 0
    ? "No rules yet. Create the first one with + New rule."
    : "No rules match these filters. Clear the search or pick another table or status.";
  selectAll.checked = shown.length > 0 && shown.every((rule) => selected.has(rule.id));
}

function fillFilters(): void {
  const tables = [...new Set(rules.flatMap((rule) => ruleTables(rule)))]
    .sort((a, b) => (tableNames.get(a) ?? a).localeCompare(tableNames.get(b) ?? b));
  fillSelect(table, [{ value: "", label: "All tables" }, ...tables.map((t) => ({ value: t, label: tableNames.get(t) ?? t }))]);
}

async function refresh(): Promise<void> {
  const statusLine = byId("status");
  const button = byId<HTMLButtonElement>("refresh");
  button.disabled = true;
  setStatus(statusLine, "Loading rules…");
  try {
    rules = await loadRules();
    tableNames = await tableDisplayNames(rules.flatMap((rule) => ruleTables(rule)));
    for (const id of selected) if (!rules.some((rule) => rule.id === id)) selected.delete(id);
    fillFilters();
    render();
    updateSelection();
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the rules", error), "error");
  } finally {
    button.disabled = false;
  }
}

async function openEditor(params: Record<string, string>): Promise<void> {
  try {
    await openRecord("rule-editor", params);
  } catch (error) {
    setStatus(byId("status"), describeError("open the rule editor", error), "error");
  }
}

async function openPublish(): Promise<void> {
  try {
    await openDialog("publish-release", { ruleIds: [...selected].join(",") });
  } catch (error) {
    setStatus(byId("status"), describeError("open the publish dialog", error), "error");
  }
  await refresh();
}

function applyIncomingParams(): void {
  const params = pageParams();
  if (params.status) status.value = params.status;
}

function start(): void {
  initWebApi();
  author = applyRole();
  for (const control of [search, table, status]) control.addEventListener("input", render);
  selectAll.addEventListener("change", () => {
    for (const rule of rules.filter((r) => matches(r, filters()))) {
      if (selectAll.checked) selected.add(rule.id);
      else selected.delete(rule.id);
    }
    render();
    updateSelection();
  });
  byId("refresh").addEventListener("click", () => void refresh());
  byId("newRule").addEventListener("click", () => void openEditor({ mode: "new" }));
  byId("publishSelected").addEventListener("click", () => void openPublish());
  void refresh().then(() => {
    applyIncomingParams();
    render();
  });
}

start();
