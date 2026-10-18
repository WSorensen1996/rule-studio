// Table, column and relationship metadata for the field-path picker and typed value inputs.
// Read from the Web API metadata endpoints and cached for the lifetime of the page.
import { webApiUrl } from "./xrm";

/** How a value for this column is entered and stored (json-formats.md, Fixed values). */
export type ValueKind = "text" | "number" | "boolean" | "choice" | "lookup" | "date";

export interface ChoiceOption {
  value: number;
  label: string;
}

export interface ColumnInfo {
  logicalName: string;
  label: string;
  attributeType: string;
  kind: ValueKind;
  /** Tables a lookup column can point to. */
  targets: string[];
  options: ChoiceOption[];
  writable: boolean;
}

/** A one-to-many relationship seen from the parent: child rows of `table` point back through `attribute`. */
export interface ChildRelation {
  table: string;
  attribute: string;
}

export interface TableInfo {
  logicalName: string;
  label: string;
  columns: ColumnInfo[];
  children: ChildRelation[];
}

export interface TableName {
  logicalName: string;
  label: string;
}

interface Label {
  UserLocalizedLabel?: { Label?: string } | null;
}

interface AttributeRow {
  LogicalName: string;
  DisplayName?: Label;
  AttributeType: string;
  AttributeOf: string | null;
  IsValidForRead: boolean;
  IsValidForUpdate: boolean;
}

const SKIPPED_TYPES = new Set(["Virtual", "EntityName", "ManagedProperty", "CalendarRules", "PartyList"]);
const KINDS: Record<string, ValueKind> = {
  Integer: "number", BigInt: "number", Decimal: "number", Double: "number", Money: "number",
  Boolean: "boolean", Picklist: "choice", State: "choice", Status: "choice",
  Lookup: "lookup", Customer: "lookup", Owner: "lookup", DateTime: "date",
};

const tables = new Map<string, Promise<TableInfo>>();
let tableList: Promise<TableName[]> | undefined;

function label(value: Label | undefined, fallback: string): string {
  return value?.UserLocalizedLabel?.Label ?? fallback;
}

async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(`${webApiUrl()}${path}`, {
    headers: { Accept: "application/json", "OData-Version": "4.0", "OData-MaxVersion": "4.0" },
    credentials: "same-origin",
  });
  if (!response.ok) throw new Error(`Metadata request failed (${response.status}): ${path}`);
  return (await response.json()) as T;
}

function definition(logicalName: string): string {
  return `EntityDefinitions(LogicalName='${encodeURIComponent(logicalName)}')`;
}

async function fetchTable(logicalName: string): Promise<TableInfo> {
  const base = definition(logicalName);
  const [table, lookups, picklists] = await Promise.all([
    getJson<{ DisplayName?: Label; PrimaryIdAttribute: string; Attributes: AttributeRow[]; OneToManyRelationships: { ReferencingEntity: string; ReferencingAttribute: string }[] }>(
      `${base}?$select=LogicalName,DisplayName,PrimaryIdAttribute&$expand=Attributes($select=LogicalName,DisplayName,AttributeType,AttributeOf,IsValidForRead,IsValidForUpdate),OneToManyRelationships($select=ReferencingEntity,ReferencingAttribute)`,
    ),
    getJson<{ value: { LogicalName: string; Targets: string[] | null }[] }>(
      `${base}/Attributes/Microsoft.Dynamics.CRM.LookupAttributeMetadata?$select=LogicalName,Targets`,
    ),
    getJson<{ value: { LogicalName: string; OptionSet?: { Options: { Value: number; Label?: Label }[] } | null }[] }>(
      `${base}/Attributes/Microsoft.Dynamics.CRM.PicklistAttributeMetadata?$select=LogicalName&$expand=OptionSet($select=Options)`,
    ),
  ]);
  const targets = new Map(lookups.value.map((row) => [row.LogicalName, row.Targets ?? []]));
  const options = new Map(picklists.value.map((row) => [
    row.LogicalName,
    (row.OptionSet?.Options ?? []).map((o) => ({ value: o.Value, label: label(o.Label, String(o.Value)) })),
  ]));
  const columns = table.Attributes
    .filter((a) => a.IsValidForRead && !a.AttributeOf && !SKIPPED_TYPES.has(a.AttributeType))
    .map((a): ColumnInfo => ({
      logicalName: a.LogicalName,
      label: label(a.DisplayName, a.LogicalName),
      attributeType: a.AttributeType,
      kind: KINDS[a.AttributeType] ?? "text",
      targets: targets.get(a.LogicalName) ?? [],
      options: options.get(a.LogicalName) ?? [],
      writable: a.IsValidForUpdate,
    }))
    .sort((a, b) => a.label.localeCompare(b.label));
  // Skip relationships keyed on this table's own id (activity -> phonecall): they are the same record, not a child.
  const children = table.OneToManyRelationships
    .filter((r) => r.ReferencingAttribute !== table.PrimaryIdAttribute)
    .map((r) => ({ table: r.ReferencingEntity, attribute: r.ReferencingAttribute }))
    .sort((a, b) => a.table.localeCompare(b.table) || a.attribute.localeCompare(b.attribute));
  return { logicalName, label: label(table.DisplayName, logicalName), columns, children };
}

/** Columns, lookups and child relationships of a table. Rejects when the table does not exist. */
export function loadTable(logicalName: string): Promise<TableInfo> {
  let table = tables.get(logicalName);
  if (!table) {
    table = fetchTable(logicalName);
    // Forget failures so a later attempt (after a typo is fixed) asks again.
    table.catch(() => tables.delete(logicalName));
    tables.set(logicalName, table);
  }
  return table;
}

/** Every table with a display name, for the target-table picker. */
export function loadTableList(): Promise<TableName[]> {
  tableList ??= getJson<{ value: { LogicalName: string; DisplayName?: Label; IsIntersect?: boolean }[] }>(
    "EntityDefinitions?$select=LogicalName,DisplayName,IsIntersect",
  ).then((result) => result.value
    .filter((t) => !t.IsIntersect && t.DisplayName?.UserLocalizedLabel?.Label)
    .map((t) => ({ logicalName: t.LogicalName, label: label(t.DisplayName, t.LogicalName) }))
    .sort((a, b) => a.label.localeCompare(b.label)));
  return tableList;
}

/** Display name of a table from the cached list; the logical name when unknown. */
export async function tableLabel(logicalName: string): Promise<string> {
  try {
    const list = await loadTableList();
    return list.find((t) => t.logicalName === logicalName)?.label ?? logicalName;
  } catch {
    return logicalName;
  }
}

/** The column, or undefined when the table or column cannot be found. */
export async function findColumn(table: string, column: string): Promise<ColumnInfo | undefined> {
  try {
    return (await loadTable(table)).columns.find((c) => c.logicalName === column);
  } catch {
    return undefined;
  }
}
