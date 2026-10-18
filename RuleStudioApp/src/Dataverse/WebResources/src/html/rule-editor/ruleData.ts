// Loads a rule, its version list and one version's authoring rows into the editor model.
import { Filter, XrmQuery } from "@delegateas/xrmquery";


import { ActionType, Aggregate, Branch, emptyValue, Logical, newVersion, Operator } from "./model";
import { parsePath } from "./paths";
import { uniqueBy } from "../../shared/data/unique";

import type { ActionNode, GroupNode, MappingNode, RuleModel, ValueModel, VersionModel } from "./model";

export interface VersionListItem {
  id: string;
  versionNumber: number;
  status: number | null;
  statusLabel: string;
  changeNote: string;
  priority: number | null;
  publishedOn: string;
  publishedBy: string;
}

interface ValueRow {
  rs_valuesourcetype: number | null;
  rs_value: string | null;
  rs_valuepath: string | null;
  rs_lookupdefinitionid_guid?: string | null;
  rs_customfunctionid_guid?: string | null;
}

function toValue(row: ValueRow): ValueModel {
  return {
    ...emptyValue(),
    sourceType: row.rs_valuesourcetype ?? emptyValue().sourceType,
    value: row.rs_value ?? "",
    valuePath: parsePath(row.rs_valuepath),
    lookupDefinitionId: row.rs_lookupdefinitionid_guid ?? null,
    customFunctionId: row.rs_customfunctionid_guid ?? null,
  };
}

export async function loadRule(ruleId: string): Promise<RuleModel> {
  const row = await XrmQuery.retrieve((x) => x.rs_rules, ruleId)
    .select((x) => [x.rs_name, x.rs_rulekey, x.rs_description, x.rs_enabled, x.rs_draftversionid_guid, x.rs_publishedversionid_guid])
    .execute();
  return {
    id: ruleId,
    isNew: false,
    name: row.rs_name ?? "",
    ruleKey: row.rs_rulekey ?? "",
    description: row.rs_description ?? "",
    enabled: row.rs_enabled !== false,
    draftVersionId: row.rs_draftversionid_guid,
    publishedVersionId: row.rs_publishedversionid_guid,
  };
}

/** Every version of the rule, newest first. */
export async function loadVersionList(ruleId: string): Promise<VersionListItem[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_ruleversions)
    .select((x) => [x.rs_ruleversionid, x.rs_versionnumber, x.rs_status, x.rs_changenote, x.rs_priority, x.rs_publishedon, x.rs_publishedby_guid])
    .filter((x) => Filter.equals(x.rs_ruleid_guid, Filter.makeGuid(ruleId)))
    .orderDesc((x) => x.rs_versionnumber)
    .includeFormattedValues()
    .execute();
  return rows.map((row) => ({
    id: row.rs_ruleversionid,
    versionNumber: row.rs_versionnumber ?? 0,
    status: row.rs_status,
    statusLabel: row.rs_status_formatted ?? "",
    changeNote: row.rs_changenote ?? "",
    priority: row.rs_priority,
    publishedOn: row.rs_publishedon_formatted ?? "",
    publishedBy: row.rs_publishedby_formatted ?? "",
  }));
}

type MappingRow = Pick<XDT.rs_ActionInputMapping_Result, "rs_actionid_guid" | "rs_conditionid_guid" | "rs_parametername" | "rs_valuesourcetype" | "rs_value" | "rs_valuepath"> & {
  rs_actioninputmappingid: string;
};

async function loadMappings(conditionIds: string[], actionIds: string[]): Promise<MappingRow[]> {
  if (conditionIds.length + actionIds.length === 0) return [];
  // In() takes PropertyValues as a JSON string array, so ids must be quoted strings, not bare makeGuid values.
  const guids = (ids: string[]): XQW.Guid[] => [...new Set(ids)] as unknown as XQW.Guid[];
  return XrmQuery.retrieveMultiple((x) => x.rs_actioninputmappings)
    .select((x) => [x.rs_actioninputmappingid, x.rs_actionid_guid, x.rs_conditionid_guid, x.rs_parametername, x.rs_valuesourcetype, x.rs_value, x.rs_valuepath])
    // An empty In() list is rejected by the Web API, so each side is only added when it has ids.
    .filter((x) => Filter.ors([
      ...(conditionIds.length ? [Filter.$in(x.rs_conditionid_guid, guids(conditionIds))] : []),
      ...(actionIds.length ? [Filter.$in(x.rs_actionid_guid, guids(actionIds))] : []),
    ]))
    .execute();
}

function mappingsFor(rows: MappingRow[], ownerId: string): MappingNode[] {
  return rows
    .filter((m) => m.rs_conditionid_guid === ownerId || m.rs_actionid_guid === ownerId)
    .map((m) => ({ id: m.rs_actioninputmappingid, isNew: false, parameterName: m.rs_parametername ?? "", value: toValue(m) }));
}

type GroupRow = XDT.rs_ConditionGroup_Result;
type ConditionRow = XDT.rs_Condition_Result;

function buildGroup(row: GroupRow, groups: GroupRow[], conditions: ConditionRow[], mappings: MappingRow[]): GroupNode {
  const childGroups = groups.filter((g) => g.rs_parentgroupid_guid === row.rs_conditiongroupid)
    .map((g) => ({ order: g.rs_order ?? 0, node: buildGroup(g, groups, conditions, mappings) }));
  const childConditions = conditions.filter((c) => c.rs_conditiongroupid_guid === row.rs_conditiongroupid)
    .map((c) => ({
      order: c.rs_order ?? 0,
      node: {
        kind: "condition" as const, id: c.rs_conditionid, isNew: false, fieldPath: parsePath(c.rs_fieldpath),
        aggregate: c.rs_aggregate ?? Aggregate.None, operator: c.rs_operator ?? Operator.Equals,
        value: toValue(c), mappings: mappingsFor(mappings, c.rs_conditionid),
      },
    }));
  return {
    kind: "group", id: row.rs_conditiongroupid, isNew: false, operator: row.rs_logicaloperator ?? Logical.AND,
    children: [...childGroups, ...childConditions].sort((a, b) => a.order - b.order).map((c) => c.node),
  };
}

async function loadRows(versionId: string): Promise<{ groups: GroupRow[]; actions: XDT.rs_Action_Result[] }> {
  const [groups, actions] = await Promise.all([
    XrmQuery.retrieveMultiple((x) => x.rs_conditiongroups)
      .select((x) => [x.rs_conditiongroupid, x.rs_parentgroupid_guid, x.rs_logicaloperator, x.rs_order, x.rs_aggregatescopeconditionid_guid])
      .expand((x) => x.rs_rs_conditiongroup_rs_condition_rs_conditiongroupid, (c) => [
        c.rs_conditionid, c.rs_conditiongroupid_guid, c.rs_order, c.rs_fieldpath, c.rs_aggregate, c.rs_operator,
        c.rs_valuesourcetype, c.rs_value, c.rs_valuepath, c.rs_lookupdefinitionid_guid, c.rs_customfunctionid_guid,
      ])
      .filter((x) => Filter.equals(x.rs_ruleversionid_guid, Filter.makeGuid(versionId)))
      .execute(),
    XrmQuery.retrieveMultiple((x) => x.rs_actions)
      .select((x) => [x.rs_actionid, x.rs_branch, x.rs_order, x.rs_actiontype, x.rs_targetfield, x.rs_valuesourcetype, x.rs_value,
        x.rs_valuepath, x.rs_lookupdefinitionid_guid, x.rs_customfunctionid_guid, x.rs_message, x.rs_messagelevel])
      .filter((x) => Filter.equals(x.rs_ruleversionid_guid, Filter.makeGuid(versionId)))
      .orderAsc((x) => x.rs_order)
      .execute(),
  ]);
  return { groups: groups as GroupRow[], actions: actions as XDT.rs_Action_Result[] };
}

function toAction(row: XDT.rs_Action_Result, mappings: MappingRow[]): ActionNode {
  return {
    id: row.rs_actionid, isNew: false, branch: row.rs_branch ?? Branch.Then, actionType: row.rs_actiontype ?? ActionType.Setfield,
    targetField: row.rs_targetfield ?? "", value: toValue(row), message: row.rs_message ?? "",
    messageLevel: row.rs_messagelevel, mappings: mappingsFor(mappings, row.rs_actionid),
  };
}

function or<T>(value: T | null | undefined, fallback: T): T {
  return value ?? fallback;
}

function applyHeader(model: VersionModel, row: XDT.rs_RuleVersion_Result): void {
  Object.assign(model, {
    versionNumber: or(row.rs_versionnumber, model.versionNumber),
    status: or(row.rs_status, model.status),
    derivedFromId: row.rs_derivedfromversionid_guid,
    targetTable: or(row.rs_targettable, ""),
    event: or(row.rs_event, model.event),
    stage: or(row.rs_stage, model.stage),
    filteredColumns: or(row.rs_filteredcolumns, ""),
    priority: or(row.rs_priority, model.priority),
    stopProcessing: row.rs_stopprocessing === true,
    failurePolicy: or(row.rs_failurepolicy, model.failurePolicy),
    runAs: or(row.rs_runas, model.runAs),
    retryCount: or(row.rs_retrycount, 0),
    retryDelayMs: or(row.rs_retrydelayms, 0),
    logLevel: row.rs_loglevel ?? null,
    changeNote: or(row.rs_changenote, ""),
  } satisfies Partial<VersionModel>);
}

/** One version with its condition tree and actions. Aggregate-scope groups are not shown and stay untouched. */
export async function loadVersion(versionId: string): Promise<VersionModel> {
  const [header, rows] = await Promise.all([
    XrmQuery.retrieve((x) => x.rs_ruleversions, versionId).execute(),
    loadRows(versionId),
  ]);
  const groups = rows.groups.filter((g) => !g.rs_aggregatescopeconditionid_guid);
  const conditions = uniqueBy(groups.flatMap((g) => g.rs_rs_conditiongroup_rs_condition_rs_conditiongroupid ?? []), (c) => c.rs_conditionid);
  const mappings = await loadMappings(conditions.map((c) => c.rs_conditionid), rows.actions.map((a) => a.rs_actionid));

  const model = newVersion(0);
  model.id = versionId;
  model.isNew = false;
  applyHeader(model, header);
  const roots = groups.filter((g) => !g.rs_parentgroupid_guid).sort((a, b) => (a.rs_order ?? 0) - (b.rs_order ?? 0));
  // One root group per version; extra roots (not created by this editor) are nested under the first.
  if (roots.length > 0) {
    const [first, ...rest] = roots.map((r) => buildGroup(r, groups, conditions, mappings));
    model.root = first;
    model.root.children.push(...rest);
  } else {
    model.root.isNew = true;
  }
  model.actions = rows.actions.map((a) => toAction(a, mappings));
  return model;
}
