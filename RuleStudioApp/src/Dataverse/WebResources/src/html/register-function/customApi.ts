// The environment's Custom API catalog (customapi, customapirequestparameter,
// customapiresponseproperty) and how its parameter types map to Rule Studio parameter types.
import { getJson } from "./webApi";

import type { FunctionParameter, ParameterType } from "../../shared/formats";

export interface CustomApiInfo {
  id: string;
  uniqueName: string;
  displayName: string;
  description: string;
}

export interface ApiShape {
  parameters: FunctionParameter[];
  /** Request parameters whose Custom API type Rule Studio cannot map, as "name (type)". */
  unsupported: string[];
  /** Mapped type of the first response property, when there is one Rule Studio supports. */
  output: ParameterType | null;
}

/** Custom API type codes. 3 Entity, 4 EntityCollection, 9 PickList, 11 StringArray, 12 Guid are not mapped. */
const TYPE_MAP = new Map<number, ParameterType>([
  [0, "boolean"], [1, "date"], [2, "decimal"], [5, "lookup"], [6, "decimal"], [7, "number"], [8, "money"], [10, "text"],
]);

const TYPE_NAMES = ["Boolean", "DateTime", "Decimal", "Entity", "EntityCollection", "EntityReference", "Float",
  "Integer", "Money", "Picklist", "String", "StringArray", "Guid"];

function typeName(code: number): string {
  return TYPE_NAMES[code] ?? `type ${code}`;
}

export async function loadCustomApis(): Promise<CustomApiInfo[]> {
  const result = await getJson<{ value: { customapiid: string; uniquename: string; displayname: string | null; description: string | null }[] }>(
    "customapis?$select=customapiid,uniquename,displayname,description&$orderby=uniquename",
  );
  return result.value.map((row) => ({
    id: row.customapiid,
    uniqueName: row.uniquename,
    displayName: row.displayname ?? row.uniquename,
    description: row.description ?? "",
  }));
}

interface RequestRow {
  uniquename: string;
  displayname: string | null;
  description: string | null;
  type: number;
  isoptional: boolean;
  logicalentityname: string | null;
}

function toParameter(row: RequestRow): FunctionParameter {
  const type = TYPE_MAP.get(row.type) ?? "text";
  const parameter: FunctionParameter = { name: row.uniquename, type, required: !row.isoptional };
  if (row.displayname && row.displayname !== row.uniquename) parameter.displayName = row.displayname;
  if (row.description) parameter.description = row.description;
  if (type === "lookup" && row.logicalentityname) parameter.table = row.logicalentityname;
  return parameter;
}

export async function loadApiShape(customApiId: string): Promise<ApiShape> {
  const filter = `$filter=_customapiid_value eq ${customApiId}`;
  const [requests, responses] = await Promise.all([
    getJson<{ value: RequestRow[] }>(`customapirequestparameters?$select=uniquename,displayname,description,type,isoptional,logicalentityname&${filter}&$orderby=uniquename`),
    getJson<{ value: { uniquename: string; type: number }[] }>(`customapiresponseproperties?$select=uniquename,type&${filter}&$orderby=uniquename`),
  ]);
  const supported = requests.value.filter((row) => TYPE_MAP.has(row.type));
  const unsupported = requests.value.filter((row) => !TYPE_MAP.has(row.type)).map((row) => `${row.uniquename} (${typeName(row.type)})`);
  const firstOutput = responses.value.find((row) => TYPE_MAP.has(row.type));
  return {
    parameters: supported.map(toParameter),
    unsupported,
    output: firstOutput ? TYPE_MAP.get(firstOutput.type) ?? null : null,
  };
}
