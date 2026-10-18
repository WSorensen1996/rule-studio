// Record input for Test & trace: search a real record by primary name, and sample values
// (column → value) for the table under test.
import { byId, h } from "../../shared/dom";
import { describeError, setStatus } from "../../shared/notify";
import { loadTable } from "../../shared/tableMetadata";
import { webApiUrl } from "../../shared/xrm";

let record: { table: string; id: string; name: string } | null = null;

/** The chosen real record, if any. */
export function chosenRecordId(): string | undefined {
  return record?.id;
}

/** Clears the record and sample values when the table under test changes. */
export function resetRecord(table: string): void {
  if (record && record.table !== table) {
    record = null;
    showRecord();
  }
  byId("recordResults").replaceChildren();
  byId("samples").replaceChildren();
}

interface TableKeys { set: string; id: string; name: string }
const keys = new Map<string, Promise<TableKeys>>();

async function tableKeys(table: string): Promise<TableKeys> {
  let found = keys.get(table);
  if (!found) {
    found = getJson<{ EntitySetName: string; PrimaryIdAttribute: string; PrimaryNameAttribute: string }>(
      `EntityDefinitions(LogicalName='${encodeURIComponent(table)}')?$select=EntitySetName,PrimaryIdAttribute,PrimaryNameAttribute`,
    ).then((d) => ({ set: d.EntitySetName, id: d.PrimaryIdAttribute, name: d.PrimaryNameAttribute }));
    found.catch(() => keys.delete(table));
    keys.set(table, found);
  }
  return found;
}

async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(`${webApiUrl()}${path}`, {
    headers: { Accept: "application/json", "OData-Version": "4.0", "OData-MaxVersion": "4.0" },
    credentials: "same-origin",
  });
  if (!response.ok) throw new Error(`Request failed (${response.status}).`);
  return (await response.json()) as T;
}

function showRecord(): void {
  const chosen = byId("recordChosen");
  chosen.replaceChildren();
  if (!record) return;
  const clear = h("button", { className: "btn mini", attrs: { type: "button" } }, "Clear");
  clear.addEventListener("click", () => {
    record = null;
    showRecord();
  });
  chosen.append("Testing against ", h("b", {}, record.name), " ", clear);
}

export async function searchRecords(table: string): Promise<void> {
  const text = byId<HTMLInputElement>("recordSearch").value.trim();
  const results = byId("recordResults");
  if (!table) {
    results.replaceChildren(h("p", { className: "muted t11" }, "Choose a table or rule first."));
    return;
  }
  try {
    const k = await tableKeys(table);
    const filter = text ? `&$filter=contains(${k.name},'${encodeURIComponent(text.replace(/'/g, "''"))}')` : "";
    const rows = await getJson<{ value: Record<string, unknown>[] }>(`${k.set}?$select=${k.id},${k.name}&$top=10&$orderby=${k.name}${filter}`);
    const buttons = rows.value.map((row) => {
      const id = String(row[k.id]);
      const name = typeof row[k.name] === "string" && row[k.name] ? String(row[k.name]) : id;
      const button = h("button", { className: "btn mini", attrs: { type: "button" } }, name);
      button.addEventListener("click", () => {
        record = { table, id, name };
        results.replaceChildren();
        showRecord();
      });
      return button;
    });
    results.replaceChildren(...(buttons.length ? buttons : [h("p", { className: "muted t11" }, "No records found.")]));
  } catch (error) {
    results.replaceChildren(h("p", { className: "t11 trace-error" }, describeError("search records", error)));
  }
}

export async function addSample(table: string, statusLine: HTMLElement): Promise<void> {
  if (!table) {
    setStatus(statusLine, "Choose a table or rule before adding sample values.", "error");
    return;
  }
  try {
    const info = await loadTable(table);
    const column = h("select", { className: "select", attrs: { "aria-label": "Column" } },
      ...info.columns.map((c) => h("option", { attrs: { value: c.logicalName } }, `${c.label} (${c.logicalName})`)));
    const value = h("input", { className: "input", attrs: { type: "text", "aria-label": "Value", placeholder: "Value" } });
    const remove = h("button", { className: "btn mini", attrs: { type: "button", "aria-label": "Remove value" } }, "×");
    const row = h("div", { className: "sample-row" }, column, value, remove);
    remove.addEventListener("click", () => row.remove());
    byId("samples").append(row);
  } catch (error) {
    setStatus(statusLine, describeError("load the table columns", error), "error");
  }
}

function parseValue(text: string): unknown {
  const trimmed = text.trim();
  if (trimmed === "") return null;
  try {
    return JSON.parse(trimmed) as unknown;
  } catch {
    return text;
  }
}

export function readSamples(): Record<string, unknown> {
  const values: Record<string, unknown> = {};
  for (const row of Array.from(byId("samples").querySelectorAll(".sample-row"))) {
    const column = row.querySelector("select")?.value;
    const input = row.querySelector("input");
    if (column && input) values[column] = parseValue(input.value);
  }
  return values;
}
