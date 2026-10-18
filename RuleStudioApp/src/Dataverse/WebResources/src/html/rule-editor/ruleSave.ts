// Saves the editor model as one $batch changeset, so a draft is stored completely or not at all.
// Row ids are generated in the browser, which lets new children bind to new parents in the same
// changeset. Removed rows are deleted first, children before parents.
import { Branch, isEmptyValue, SourceType, VersionStatus } from "./model";
import { storePath } from "./paths";
import { rs_ruleversion_rs_source as VersionSource } from "../../../typings/XRM/OptionSets";
import { bindTo, executeChangeset } from "../../shared/batch";

import type { ActionNode, ConditionNode, EditorState, GroupNode, MappingNode, ValueModel } from "./model";
import type { ChangeRequest } from "../../shared/batch";

type Row = Record<string, unknown>;
type Binds = Record<string, string | null>;

interface Table {
  set: string;
  key: string;
}

const T = {
  rule: { set: "rs_rules", key: "rs_ruleid" },
  version: { set: "rs_ruleversions", key: "rs_ruleversionid" },
  group: { set: "rs_conditiongroups", key: "rs_conditiongroupid" },
  condition: { set: "rs_conditions", key: "rs_conditionid" },
  action: { set: "rs_actions", key: "rs_actionid" },
  mapping: { set: "rs_actioninputmappings", key: "rs_actioninputmappingid" },
} satisfies Record<string, Table>;

class ChangeSet {
  readonly requests: ChangeRequest[] = [];

  /** Creates the row with its client id, or updates it. Null binds clear a lookup on update and are left out on create. */
  write(table: Table, row: { id: string; isNew: boolean }, columns: Row, binds: Binds = {}): void {
    const body: Row = { ...columns };
    for (const [navigation, target] of Object.entries(binds)) {
      if (target || !row.isNew) body[`${navigation}@odata.bind`] = target;
    }
    if (row.isNew) {
      body[table.key] = row.id;
      this.requests.push({ method: "POST", path: table.set, body });
    } else {
      this.requests.push({ method: "PATCH", path: `${table.set}(${row.id})`, body });
    }
  }

  delete(table: Table, ids: string[]): void {
    for (const id of new Set(ids)) this.requests.push({ method: "DELETE", path: `${table.set}(${id})` });
  }
}

function valueColumns(value: ValueModel): Row {
  const text = value.sourceType === SourceType.Fixed || value.sourceType === SourceType.Expression;
  return {
    rs_valuesourcetype: value.sourceType,
    rs_value: text && value.value ? value.value : null,
    rs_valuepath: value.sourceType === SourceType.Fieldpath ? storePath(value.valuePath) : null,
  };
}

function valueBinds(value: ValueModel): Binds {
  return {
    rs_LookupDefinitionId: bindTo("rs_lookupdefinitions", value.sourceType === SourceType.Lookup ? value.lookupDefinitionId : null),
    rs_CustomFunctionId: bindTo("rs_customfunctions", value.sourceType === SourceType.Function ? value.customFunctionId : null),
  };
}

interface Owner {
  navigation: "rs_ConditionId" | "rs_ActionId";
  bind: string | null;
}

/** The changes being built, the version they belong to and mappings emptied by the author. */
interface TreeContext {
  changes: ChangeSet;
  versionId: string;
  deletions: string[];
}

function writeMappings(context: TreeContext, owner: Owner, rows: MappingNode[]): void {
  for (const m of rows) {
    if (isEmptyValue(m.value)) {
      if (!m.isNew) context.deletions.push(m.id);
      continue;
    }
    context.changes.write(T.mapping, m,
      { rs_name: m.parameterName, rs_parametername: m.parameterName, ...valueColumns(m.value) },
      { [owner.navigation]: owner.bind });
  }
}

function writeCondition(context: TreeContext, node: ConditionNode, groupId: string, order: number): void {
  context.changes.write(T.condition, node, {
    rs_name: node.fieldPath?.column ?? `Condition ${order + 1}`,
    rs_order: order,
    rs_fieldpath: storePath(node.fieldPath),
    rs_aggregate: node.aggregate,
    rs_operator: node.operator,
    ...valueColumns(node.value),
  }, { rs_ConditionGroupId: bindTo(T.group.set, groupId), ...valueBinds(node.value) });
  const mappings = node.value.sourceType === SourceType.Function ? node.mappings : [];
  writeMappings(context, { navigation: "rs_ConditionId", bind: bindTo(T.condition.set, node.id) }, mappings);
}

function writeGroup(context: TreeContext, node: GroupNode, parentId: string | null, order: number): void {
  context.changes.write(T.group, node,
    { rs_name: parentId ? `Group ${order + 1}` : "Root", rs_order: order, rs_logicaloperator: node.operator },
    { rs_RuleVersionId: bindTo(T.version.set, context.versionId), rs_ParentGroupId: bindTo(T.group.set, parentId) });
  node.children.forEach((child, index) => {
    if (child.kind === "group") writeGroup(context, child, node.id, index);
    else writeCondition(context, child, node.id, index);
  });
}

function writeAction(context: TreeContext, node: ActionNode, order: number): void {
  context.changes.write(T.action, node, {
    rs_name: `${node.branch === Branch.Else ? "Else" : "Then"} ${order + 1}`,
    rs_branch: node.branch,
    rs_order: order,
    rs_actiontype: node.actionType,
    rs_targetfield: node.targetField || null,
    ...valueColumns(node.value),
    rs_message: node.message || null,
    rs_messagelevel: node.messageLevel,
  }, { rs_RuleVersionId: bindTo(T.version.set, context.versionId), ...valueBinds(node.value) });
  const mappings = node.value.sourceType === SourceType.Function ? node.mappings : [];
  writeMappings(context, { navigation: "rs_ActionId", bind: bindTo(T.action.set, node.id) }, mappings);
}

function writeRule(changes: ChangeSet, state: EditorState): void {
  const { rule } = state;
  changes.write(T.rule, rule, {
    rs_name: rule.name, rs_rulekey: rule.ruleKey, rs_description: rule.description || null, rs_enabled: rule.enabled,
  });
}

function writeVersion(changes: ChangeSet, state: EditorState): void {
  const { rule, version } = state;
  const columns: Row = {
    rs_name: `${rule.ruleKey} v${version.versionNumber}`,
    rs_targettable: version.targetTable || null, rs_event: version.event, rs_stage: version.stage,
    rs_filteredcolumns: version.filteredColumns || null, rs_priority: version.priority, rs_stopprocessing: version.stopProcessing,
    rs_failurepolicy: version.failurePolicy, rs_runas: version.runAs, rs_retrycount: version.retryCount,
    rs_retrydelayms: version.retryDelayMs, rs_loglevel: version.logLevel, rs_changenote: version.changeNote || null,
  };
  const binds: Binds = {};
  if (version.isNew) {
    Object.assign(columns, { rs_versionnumber: version.versionNumber, rs_status: VersionStatus.Draft, rs_source: VersionSource.Authored });
    binds.rs_RuleId = bindTo(T.rule.set, rule.id);
    binds.rs_DerivedFromVersionId = bindTo(T.version.set, version.derivedFromId);
  }
  changes.write(T.version, version, columns, binds);
}

function writeTree(state: EditorState): ChangeSet {
  const context: TreeContext = { changes: new ChangeSet(), versionId: state.version.id, deletions: [] };
  writeGroup(context, state.version.root, null, 0);
  for (const branch of [Branch.Then, Branch.Else]) {
    state.version.actions.filter((a) => a.branch === branch).forEach((a, index) => writeAction(context, a, index));
  }
  // Mappings emptied by the author are removed together with the explicitly removed ones.
  state.removed.mappings.push(...context.deletions);
  return context.changes;
}

/** Saves the rule and, when it changed, the version. A new version becomes the rule's draft. */
export async function saveState(state: EditorState): Promise<void> {
  const changes = new ChangeSet();
  const saveVersion = state.versionDirty;
  const tree = saveVersion ? writeTree(state) : new ChangeSet();
  const { removed } = state;
  changes.delete(T.mapping, removed.mappings);
  changes.delete(T.condition, removed.conditions);
  changes.delete(T.group, removed.groups);
  changes.delete(T.action, removed.actions);
  writeRule(changes, state);
  if (saveVersion) {
    writeVersion(changes, state);
    changes.requests.push(...tree.requests);
    if (state.version.isNew) {
      if (state.replacesDraftId) changes.write(T.version, { id: state.replacesDraftId, isNew: false }, { rs_status: VersionStatus.Discarded });
      changes.write(T.rule, { id: state.rule.id, isNew: false }, {}, { rs_DraftVersionId: bindTo(T.version.set, state.version.id) });
    }
  }
  await executeChangeset(changes.requests);
}

/** Marks the draft as Discarded (kept in history) and unlinks it from the rule, so the live version shows again. */
export async function discardDraft(ruleId: string, draftId: string): Promise<void> {
  const changes = new ChangeSet();
  changes.write(T.version, { id: draftId, isNew: false }, { rs_status: VersionStatus.Discarded });
  changes.write(T.rule, { id: ruleId, isNew: false }, {}, { rs_DraftVersionId: null });
  await executeChangeset(changes.requests);
}

/** Deletes a rule that is not live. Dataverse deletes its versions, conditions and actions with it. */
export async function deleteRule(ruleId: string): Promise<void> {
  const changes = new ChangeSet();
  changes.delete(T.rule, [ruleId]);
  await executeChangeset(changes.requests);
}
