// Dataverse table metadata for friendly labels. Rules store logical names (quote, account);
// the UI shows display names. Results are cached for the lifetime of the page.
import { webApiUrl } from "./xrm";

interface EntityDefinition {
  LogicalName: string;
  DisplayName?: { UserLocalizedLabel?: { Label?: string } | null };
}

const tableNames = new Map<string, Promise<string>>();

async function fetchTableName(logicalName: string): Promise<string> {
  const url = `${webApiUrl()}EntityDefinitions(LogicalName='${encodeURIComponent(logicalName)}')?$select=LogicalName,DisplayName`;
  const response = await fetch(url, { headers: { Accept: "application/json", "OData-Version": "4.0" }, credentials: "same-origin" });
  // A table that no longer exists still shows its logical name; publish validation reports it.
  if (!response.ok) return logicalName;
  const definition = (await response.json()) as EntityDefinition;
  return definition.DisplayName?.UserLocalizedLabel?.Label ?? logicalName;
}

/** Display name of a table, e.g. "Quote" for quote. Falls back to the logical name. */
export function tableDisplayName(logicalName: string): Promise<string> {
  let name = tableNames.get(logicalName);
  if (!name) {
    name = fetchTableName(logicalName).catch(() => logicalName);
    tableNames.set(logicalName, name);
  }
  return name;
}

/** Display names for several tables at once, as a lookup map. */
export async function tableDisplayNames(logicalNames: Iterable<string>): Promise<Map<string, string>> {
  const unique = [...new Set(logicalNames)];
  const names = await Promise.all(unique.map((name) => tableDisplayName(name)));
  return new Map(unique.map((name, index) => [name, names[index] ?? name]));
}
