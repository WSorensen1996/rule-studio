// Reads registered Custom Functions and where rules use them.
import { XrmQuery } from "@delegateas/xrmquery";

import { getJson } from "./webApi";
import { rs_customfunction_rs_outputtype as OutputType } from "../../../typings/XRM/OptionSets";
import { parseDocument } from "../../shared/formats";

import type { FunctionParameter, InputSchema } from "../../shared/formats";

export interface FunctionRecord {
  id: string;
  name: string;
  apiName: string;
  description: string;
  enabled: boolean;
  hasSideEffects: boolean;
  safeToRetry: boolean;
  timeoutMs: number | null;
  outputType: number | null;
  /** Null when the schema is missing or in a format this page does not know. */
  parameters: FunctionParameter[] | null;
}

export interface Usage {
  versions: number;
  rules: number;
}

const OUTPUT_LABELS = new Map<number, string>(Object.entries(OutputType).map(([label, value]) => [value, label]));

export function outputLabel(value: number | null): string {
  return value === null ? "—" : OUTPUT_LABELS.get(value) ?? String(value);
}

export async function loadFunctions(): Promise<FunctionRecord[]> {
  const rows = await XrmQuery.retrieveMultiple((x) => x.rs_customfunctions)
    .select((x) => [x.rs_customfunctionid, x.rs_name, x.rs_customapiname, x.rs_description, x.rs_enabled,
      x.rs_hassideeffects, x.rs_safetoretry, x.rs_timeoutms, x.rs_outputtype, x.rs_inputschema])
    .orderAsc((x) => x.rs_name)
    .execute();
  return rows.map((row) => {
    const schema = parseDocument<InputSchema>(row.rs_inputschema);
    return {
      id: row.rs_customfunctionid,
      name: row.rs_name ?? row.rs_customapiname ?? "Unnamed function",
      apiName: row.rs_customapiname ?? "",
      description: row.rs_description ?? "",
      enabled: row.rs_enabled !== false,
      hasSideEffects: row.rs_hassideeffects === true,
      safeToRetry: row.rs_safetoretry === true,
      timeoutMs: row.rs_timeoutms,
      outputType: row.rs_outputtype,
      parameters: schema.ok ? schema.value.parameters : null,
    };
  });
}

interface VersionRef { _rs_ruleid_value?: string | null }
interface ActionRow { _rs_customfunctionid_value: string; _rs_ruleversionid_value: string | null; rs_RuleVersionId?: VersionRef | null }
interface ConditionRow {
  _rs_customfunctionid_value: string;
  rs_ConditionGroupId?: { _rs_ruleversionid_value: string | null; rs_RuleVersionId?: VersionRef | null } | null;
}

class UsageCollector {
  private readonly versions = new Map<string, Set<string>>();
  private readonly rules = new Map<string, Set<string>>();

  add(functionId: string, versionId: string | null | undefined, ruleId: string | null | undefined): void {
    if (versionId) this.bucket(this.versions, functionId).add(versionId);
    if (ruleId) this.bucket(this.rules, functionId).add(ruleId);
  }

  result(): Map<string, Usage> {
    const usage = new Map<string, Usage>();
    for (const id of new Set([...this.versions.keys(), ...this.rules.keys()])) {
      usage.set(id, { versions: this.versions.get(id)?.size ?? 0, rules: this.rules.get(id)?.size ?? 0 });
    }
    return usage;
  }

  private bucket(map: Map<string, Set<string>>, key: string): Set<string> {
    let set = map.get(key);
    if (!set) map.set(key, (set = new Set()));
    return set;
  }
}

/** Distinct rule versions and rules whose actions or conditions call each function. */
export async function loadUsage(): Promise<Map<string, Usage>> {
  const filter = "$filter=_rs_customfunctionid_value ne null";
  const [actions, conditions] = await Promise.all([
    getJson<{ value: ActionRow[] }>(`rs_actions?$select=_rs_customfunctionid_value,_rs_ruleversionid_value&${filter}`
      + "&$expand=rs_RuleVersionId($select=_rs_ruleid_value)"),
    getJson<{ value: ConditionRow[] }>(`rs_conditions?$select=_rs_customfunctionid_value&${filter}`
      + "&$expand=rs_ConditionGroupId($select=_rs_ruleversionid_value;$expand=rs_RuleVersionId($select=_rs_ruleid_value))"),
  ]);
  const collector = new UsageCollector();
  for (const row of actions.value) {
    collector.add(row._rs_customfunctionid_value, row._rs_ruleversionid_value, row.rs_RuleVersionId?._rs_ruleid_value);
  }
  for (const row of conditions.value) {
    const group = row.rs_ConditionGroupId;
    collector.add(row._rs_customfunctionid_value, group?._rs_ruleversionid_value, group?.rs_RuleVersionId?._rs_ruleid_value);
  }
  return collector.result();
}
