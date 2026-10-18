// Loads and saves one rs_customfunction row.
import { XrmQuery } from "@delegateas/xrmquery";

import { writeRow } from "./webApi";
import { rs_customfunction_rs_outputtype as OutputType } from "../../../typings/XRM/OptionSets";
import { FORMAT_VERSION, parseDocument } from "../../shared/formats";

import type { FunctionParameter, InputSchema, ParameterType } from "../../shared/formats";

export interface FunctionForm {
  name: string;
  apiName: string;
  description: string;
  enabled: boolean;
  hasSideEffects: boolean;
  safeToRetry: boolean;
  timeoutMs: number | null;
  output: ParameterType;
  parameters: FunctionParameter[];
  /** True when the stored schema could not be read; the grid then starts empty. */
  schemaUnreadable: boolean;
}

/** rs_outputtype choice value per parameter type (the choice has the same members). */
export const OUTPUT_VALUES: Record<ParameterType, number> = {
  text: OutputType.Text, number: OutputType.Number, decimal: OutputType.Decimal, money: OutputType.Money,
  boolean: OutputType.Boolean, date: OutputType.Date, lookup: OutputType.Lookup,
};

function outputType(value: number | null): ParameterType {
  const entry = Object.entries(OUTPUT_VALUES).find(([, choice]) => choice === value);
  return (entry?.[0] as ParameterType | undefined) ?? "text";
}

export async function loadFunction(id: string): Promise<FunctionForm> {
  const row = await XrmQuery.retrieve((x) => x.rs_customfunctions, id)
    .select((x) => [x.rs_name, x.rs_customapiname, x.rs_description, x.rs_enabled, x.rs_hassideeffects,
      x.rs_safetoretry, x.rs_timeoutms, x.rs_outputtype, x.rs_inputschema])
    .execute();
  const schema = parseDocument<InputSchema>(row.rs_inputschema);
  return {
    name: row.rs_name ?? "",
    apiName: row.rs_customapiname ?? "",
    description: row.rs_description ?? "",
    enabled: row.rs_enabled !== false,
    hasSideEffects: row.rs_hassideeffects === true,
    safeToRetry: row.rs_safetoretry === true,
    timeoutMs: row.rs_timeoutms,
    output: outputType(row.rs_outputtype),
    parameters: schema.ok ? schema.value.parameters : [],
    schemaUnreadable: !schema.ok && schema.reason !== "empty",
  };
}

/** Activates or deactivates a registered function. */
export async function setFunctionEnabled(id: string, enabled: boolean): Promise<void> {
  await writeRow("PATCH", `rs_customfunctions(${id})`, { rs_enabled: enabled });
}

/** Creates the row when id is empty, otherwise updates it. */
/** Creates the function (no id) or updates it; resolves with its id. */
export async function saveFunction(id: string, form: FunctionForm): Promise<string> {
  const schema: InputSchema = { version: FORMAT_VERSION, parameters: form.parameters };
  const body = {
    rs_name: form.name,
    rs_customapiname: form.apiName,
    rs_description: form.description || null,
    rs_enabled: form.enabled,
    rs_hassideeffects: form.hasSideEffects,
    rs_safetoretry: form.safeToRetry,
    rs_timeoutms: form.timeoutMs,
    rs_outputtype: OUTPUT_VALUES[form.output],
    rs_inputschema: JSON.stringify(schema),
  };
  if (id) {
    await writeRow("PATCH", `rs_customfunctions(${id})`, body);
    return id;
  }
  return writeRow("POST", "rs_customfunctions", body);
}
