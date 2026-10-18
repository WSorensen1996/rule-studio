// The rule editor's in-memory model: one rule and the version being edited, as a tree that
// mirrors the authoring rows. Order is the array order; ruleSave.ts turns it back into rows.
import {
  rs_action_rs_actiontype as ActionType,
  rs_action_rs_branch as Branch,
  rs_condition_rs_aggregate as Aggregate,
  rs_condition_rs_operator as Operator,
  rs_conditiongroup_rs_logicaloperator as Logical,
  rs_event as RuleEvent,
  rs_loglevel as LogLevel,
  rs_ruleversion_rs_failurepolicy as FailurePolicy,
  rs_ruleversion_rs_runas as RunAs,
  rs_ruleversion_rs_status as VersionStatus,
  rs_stage as Stage,
  rs_valuesourcetype as SourceType,
} from "../../../typings/XRM/OptionSets";

import type { FieldPath } from "../../shared/formats";

/** Where a value comes from: the columns rs_valuesourcetype, rs_value, rs_valuepath and the two lookups. */
export interface ValueModel {
  sourceType: number;
  /** Fixed: JSON of the fixed value (an array for In list and Between). Expression: the NCalc text. */
  value: string;
  valuePath: FieldPath | null;
  lookupDefinitionId: string | null;
  customFunctionId: string | null;
}

export interface MappingNode {
  id: string;
  isNew: boolean;
  parameterName: string;
  value: ValueModel;
}

export interface ConditionNode {
  kind: "condition";
  id: string;
  isNew: boolean;
  fieldPath: FieldPath | null;
  aggregate: number;
  operator: number;
  value: ValueModel;
  /** Inputs when the value comes from a function. */
  mappings: MappingNode[];
}

export interface GroupNode {
  kind: "group";
  id: string;
  isNew: boolean;
  operator: number;
  children: (GroupNode | ConditionNode)[];
}

export interface ActionNode {
  id: string;
  isNew: boolean;
  branch: number;
  actionType: number;
  targetField: string;
  value: ValueModel;
  message: string;
  messageLevel: number | null;
  mappings: MappingNode[];
}

export interface VersionModel {
  id: string;
  isNew: boolean;
  versionNumber: number;
  status: number;
  derivedFromId: string | null;
  targetTable: string;
  event: number;
  stage: number;
  filteredColumns: string;
  priority: number;
  stopProcessing: boolean;
  failurePolicy: number;
  runAs: number;
  retryCount: number;
  retryDelayMs: number;
  /** null: Errors only. */
  logLevel: number | null;
  changeNote: string;
  root: GroupNode;
  actions: ActionNode[];
}

export interface RuleModel {
  id: string;
  isNew: boolean;
  name: string;
  ruleKey: string;
  description: string;
  enabled: boolean;
  draftVersionId: string | null;
  publishedVersionId: string | null;
}

/** Existing rows the author removed; deleted on save, children before parents. */
export interface Removed {
  mappings: string[];
  conditions: string[];
  groups: string[];
  actions: string[];
}

export interface EditorState {
  rule: RuleModel;
  version: VersionModel;
  removed: Removed;
  /** Set when saving creates a new version while a draft exists: that draft becomes Discarded. */
  replacesDraftId: string | null;
  ruleDirty: boolean;
  versionDirty: boolean;
}

export { ActionType, Aggregate, Branch, FailurePolicy, Logical, LogLevel, Operator, RuleEvent, RunAs, SourceType, Stage, VersionStatus };

export function newId(): string {
  return crypto.randomUUID();
}

export function emptyValue(): ValueModel {
  return { sourceType: SourceType.Fixed, value: "", valuePath: null, lookupDefinitionId: null, customFunctionId: null };
}

export function newCondition(): ConditionNode {
  return {
    kind: "condition", id: newId(), isNew: true, fieldPath: null,
    aggregate: Aggregate.None, operator: Operator.Equals, value: emptyValue(), mappings: [],
  };
}

export function newGroup(operator: number = Logical.AND): GroupNode {
  return { kind: "group", id: newId(), isNew: true, operator, children: [] };
}

export function newAction(branch: number): ActionNode {
  return {
    id: newId(), isNew: true, branch, actionType: ActionType.Setfield, targetField: "",
    value: emptyValue(), message: "", messageLevel: null, mappings: [],
  };
}

export function newMapping(parameterName: string): MappingNode {
  return { id: newId(), isNew: true, parameterName, value: emptyValue() };
}

export function newVersion(versionNumber: number): VersionModel {
  return {
    id: newId(), isNew: true, versionNumber, status: VersionStatus.Draft, derivedFromId: null,
    targetTable: "", event: RuleEvent.Update, stage: Stage.Beforesave, filteredColumns: "",
    priority: 100, stopProcessing: false, failurePolicy: FailurePolicy.Block, runAs: RunAs.Callinguser,
    retryCount: 0, retryDelayMs: 0, logLevel: null, changeNote: "",
    root: newGroup(), actions: [],
  };
}

export function newRule(): RuleModel {
  return {
    id: newId(), isNew: true, name: "", ruleKey: "", description: "", enabled: true,
    draftVersionId: null, publishedVersionId: null,
  };
}

export function noneRemoved(): Removed {
  return { mappings: [], conditions: [], groups: [], actions: [] };
}

/** True when a value has nothing in it, so no row needs to be stored for it. */
export function isEmptyValue(value: ValueModel): boolean {
  switch (value.sourceType) {
    case SourceType.Fieldpath: return !value.valuePath;
    case SourceType.Lookup: return !value.lookupDefinitionId;
    case SourceType.Function: return !value.customFunctionId;
    default: return value.value.trim() === "";
  }
}

function copyMappings(mappings: MappingNode[]): MappingNode[] {
  return mappings.map((m) => ({ ...m, id: newId(), isNew: true, value: { ...m.value } }));
}

function copyGroup(group: GroupNode): GroupNode {
  return {
    ...group, id: newId(), isNew: true,
    children: group.children.map((child) => (child.kind === "group"
      ? copyGroup(child)
      : { ...child, id: newId(), isNew: true, value: { ...child.value }, mappings: copyMappings(child.mappings) })),
  };
}

/** A new draft with the same content as `source` (create draft from live). Every row gets a new id. */
export function copyAsDraft(source: VersionModel, versionNumber: number): VersionModel {
  return {
    ...source, id: newId(), isNew: true, versionNumber, status: VersionStatus.Draft,
    derivedFromId: source.id, changeNote: "", root: copyGroup(source.root),
    actions: source.actions.map((a) => ({ ...a, id: newId(), isNew: true, value: { ...a.value }, mappings: copyMappings(a.mappings) })),
  };
}

/** Every existing row id under a group, children first, for deletion. */
export function collectRemoved(node: GroupNode | ConditionNode, removed: Removed): void {
  if (node.kind === "condition") {
    for (const m of node.mappings) if (!m.isNew) removed.mappings.push(m.id);
    if (!node.isNew) removed.conditions.push(node.id);
    return;
  }
  for (const child of node.children) collectRemoved(child, removed);
  if (!node.isNew) removed.groups.push(node.id);
}

export function countConditions(group: GroupNode): number {
  return group.children.reduce((n, c) => n + (c.kind === "group" ? countConditions(c) : 1), 0);
}
