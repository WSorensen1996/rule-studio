// Lookup definitions as the list page and the create/edit dialog need them. Also used by
// new-lookup-definition, which reads one definition and writes it back.
import { XrmQuery } from "@delegateas/xrmquery";

import { uniqueBy } from "../../shared/data/unique";
import { parseDocument } from "../../shared/formats";
import { hostXrm } from "../../shared/xrm";

import type { MatchCriteria } from "../../shared/formats";

const MAX_LOOKUPS = 5000;

export interface LookupDefinition {
  id: string;
  name: string;
  description: string;
  lookupTable: string;
  returnColumn: string;
  sortColumn: string;
  /** rs_defaultvalue: a fixed value as JSON, or empty. */
  defaultValue: string;
  /** Raw rs_matchcriteria text, kept so an unreadable document is not overwritten by accident. */
  matchCriteriaText: string;
  criteria: MatchCriteria | null;
  /** Why the criteria could not be read, when they could not. */
  criteriaProblem: string;
  noMatchPolicy: number | null;
  multiMatchPolicy: number | null;
}

export interface LookupSummary extends LookupDefinition {
  /** Conditions and actions that use this lookup (any version); they block deleting it. */
  usage: number;
}

type Row = XDT.rs_LookupDefinition_Result;

const PROBLEMS: Record<string, string> = {
  empty: "",
  invalidJson: "The match criteria are not valid JSON.",
  unsupportedVersion: "Unsupported format.",
};

function readCriteria(text: string | null): Pick<LookupDefinition, "criteria" | "criteriaProblem"> {
  const parsed = parseDocument<MatchCriteria>(text);
  return parsed.ok ? { criteria: parsed.value, criteriaProblem: "" } : { criteria: null, criteriaProblem: PROBLEMS[parsed.reason] ?? "" };
}

function toDefinition(row: Row): LookupDefinition {
  return {
    id: row.rs_lookupdefinitionid,
    name: row.rs_name ?? "Unnamed lookup",
    description: row.rs_description ?? "",
    lookupTable: row.rs_lookuptable ?? "",
    returnColumn: row.rs_returncolumn ?? "",
    sortColumn: row.rs_sortcolumn ?? "",
    defaultValue: row.rs_defaultvalue ?? "",
    matchCriteriaText: row.rs_matchcriteria ?? "",
    ...readCriteria(row.rs_matchcriteria),
    noMatchPolicy: row.rs_nomatchpolicy,
    multiMatchPolicy: row.rs_multimatchpolicy,
  };
}

/** All lookup definitions sorted by name, with how many conditions and actions use each. */
export async function loadLookups(): Promise<LookupSummary[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_lookupdefinitions)
    .select((x) => [x.rs_lookupdefinitionid, x.rs_name, x.rs_description, x.rs_lookuptable, x.rs_returncolumn,
      x.rs_sortcolumn, x.rs_defaultvalue, x.rs_matchcriteria, x.rs_nomatchpolicy, x.rs_multimatchpolicy])
    .expand((x) => x.rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid, (c) => [c.rs_conditionid])
    .expand((x) => x.rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid, (a) => [a.rs_actionid])
    .orderAsc((x) => x.rs_name)
    .top(MAX_LOOKUPS)
    .promise();
  return rows.map((row) => ({
    ...toDefinition(row as Row),
    usage: uniqueBy(row.rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid, (c) => c.rs_conditionid).length
      + uniqueBy(row.rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid, (a) => a.rs_actionid).length,
  }));
}

export async function loadLookup(id: string): Promise<LookupDefinition> {
  const row = await XrmQuery.retrieve((x) => x.rs_lookupdefinitions, id)
    .select((x) => [x.rs_lookupdefinitionid, x.rs_name, x.rs_description, x.rs_lookuptable, x.rs_returncolumn,
      x.rs_sortcolumn, x.rs_defaultvalue, x.rs_matchcriteria, x.rs_nomatchpolicy, x.rs_multimatchpolicy])
    .promise();
  return toDefinition(row as Row);
}

/** Asks before a delete; false outside the app (no dialog to ask with). */
export async function confirmDeleteLookup(name: string): Promise<boolean> {
  const xrm = hostXrm();
  if (!xrm) return false;
  const result = await xrm.Navigation.openConfirmDialog({
    title: "Delete lookup definition",
    text: `Delete "${name}"? This cannot be undone.`,
    confirmButtonLabel: "Delete",
  });
  return result.confirmed;
}

export async function deleteLookup(id: string): Promise<void> {
  await XrmQuery.deleteRecord((x) => x.rs_lookupdefinitions, id).promise();
}

export type LookupWrite = Omit<LookupDefinition, "id" | "criteria" | "criteriaProblem">;

function columns(data: LookupWrite): XDT.rs_LookupDefinition_Update {
  const text = (value: string): string | null => value.trim() || null;
  return {
    rs_name: data.name.trim(),
    rs_description: text(data.description),
    rs_lookuptable: data.lookupTable,
    rs_returncolumn: data.returnColumn,
    rs_sortcolumn: text(data.sortColumn),
    rs_defaultvalue: text(data.defaultValue),
    rs_matchcriteria: data.matchCriteriaText,
    rs_nomatchpolicy: data.noMatchPolicy as XDT.rs_LookupDefinition_Update["rs_nomatchpolicy"],
    rs_multimatchpolicy: data.multiMatchPolicy as XDT.rs_LookupDefinition_Update["rs_multimatchpolicy"],
  };
}

/** Creates the definition, or updates it when an id is given. */
/** Creates the definition (no id) or updates it; resolves with its id. */
export async function saveLookup(id: string | null, data: LookupWrite): Promise<string> {
  if (id) {
    await XrmQuery.update((x) => x.rs_lookupdefinitions, id, columns(data)).promise();
    return id;
  }
  return XrmQuery.create((x) => x.rs_lookupdefinitions, columns(data)).promise();
}
