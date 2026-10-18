// Reference data the editor offers in pickers: registered functions and lookups.
import { XrmQuery } from "@delegateas/xrmquery";

import { parseDocument } from "../../shared/formats";

import type { FunctionParameter, InputSchema, MatchCriteria } from "../../shared/formats";

export interface FunctionInfo {
  id: string;
  name: string;
  apiName: string;
  enabled: boolean;
  hasSideEffects: boolean;
  safeToRetry: boolean;
  /** Null when the schema is missing or in a format this page does not know. */
  parameters: FunctionParameter[] | null;
}

export interface LookupInfo {
  id: string;
  name: string;
  /** Table the lookup is evaluated for; only lookups for the rule's target table are offered. */
  recordTable: string | null;
  lookupTable: string;
  returnColumn: string;
}

export interface Catalog {
  functions: FunctionInfo[];
  lookups: LookupInfo[];
}

async function loadFunctions(): Promise<FunctionInfo[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_customfunctions)
    .select((x) => [x.rs_customfunctionid, x.rs_name, x.rs_customapiname, x.rs_enabled, x.rs_hassideeffects, x.rs_safetoretry, x.rs_inputschema])
    .orderAsc((x) => x.rs_name)
    .execute();
  return rows.map((row) => {
    const schema = parseDocument<InputSchema>(row.rs_inputschema);
    return {
      id: row.rs_customfunctionid,
      name: row.rs_name ?? row.rs_customapiname ?? "Unnamed function",
      apiName: row.rs_customapiname ?? "",
      enabled: row.rs_enabled !== false,
      hasSideEffects: row.rs_hassideeffects === true,
      safeToRetry: row.rs_safetoretry === true,
      parameters: schema.ok ? schema.value.parameters : null,
    };
  });
}

async function loadLookups(): Promise<LookupInfo[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_lookupdefinitions)
    .select((x) => [x.rs_lookupdefinitionid, x.rs_name, x.rs_matchcriteria, x.rs_lookuptable, x.rs_returncolumn])
    .orderAsc((x) => x.rs_name)
    .execute();
  return rows.map((row) => {
    const criteria = parseDocument<MatchCriteria>(row.rs_matchcriteria);
    return {
      id: row.rs_lookupdefinitionid,
      name: row.rs_name ?? "Unnamed lookup",
      recordTable: criteria.ok ? criteria.value.recordTable : null,
      lookupTable: row.rs_lookuptable ?? "",
      returnColumn: row.rs_returncolumn ?? "",
    };
  });
}

export async function loadCatalog(): Promise<Catalog> {
  const [functions, lookups] = await Promise.all([loadFunctions(), loadLookups()]);
  return { functions, lookups };
}
