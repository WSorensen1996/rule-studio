// One readable line per match criterion (json-formats.md §1, Display), for example
// "Product = record → Product" or "Min qty ≤ Quantity < Max qty".
import { findColumn, tableLabel } from "../../shared/tableMetadata";
import { fixedText } from "../rule-editor/fixedValue";
import { describePath } from "../rule-editor/paths";

import type { CriterionValueSource, FixedValue, MatchCriterion } from "../../shared/formats";
import type { ColumnInfo } from "../../shared/tableMetadata";

async function columnText(table: string, column: string): Promise<string> {
  return (await findColumn(table, column))?.label ?? column;
}

function choiceLabel(value: number, column: ColumnInfo | undefined): string {
  return column?.options.find((o) => o.value === value)?.label ?? String(value);
}

function fixedDisplay(value: FixedValue, column: ColumnInfo | undefined): string {
  if (typeof value === "number" && column?.kind === "choice") return choiceLabel(value, column);
  if (typeof value === "boolean") return value ? "Yes" : "No";
  if (value === null || value === "") return "(empty)";
  return typeof value === "string" ? `"${value}"` : fixedText(value);
}

/** "record → Quote → Country": the path with the record table shown as "record". */
async function pathText(source: Extract<CriterionValueSource, { type: "fieldPath" }>): Promise<string> {
  const full = await describePath(source.path);
  const start = await tableLabel(source.path.table);
  return full.startsWith(start) ? `record${full.slice(start.length)}` : full;
}

export async function valueText(source: CriterionValueSource, column?: ColumnInfo): Promise<string> {
  switch (source.type) {
    case "fixed": return fixedDisplay(source.value, column);
    case "fieldPath": return pathText(source);
    default: return `ƒ ${source.expression}`;
  }
}

async function listText(criterion: Extract<MatchCriterion, { operator: "equals" | "in" }>, table: string): Promise<string> {
  const column = await findColumn(table, criterion.column);
  const name = column?.label ?? criterion.column;
  if (criterion.operator === "equals") return `${name} = ${await valueText(criterion.value, column)}`;
  const values = await Promise.all(criterion.values.map((v) => valueText(v, column)));
  return `${name} in ${values.join(", ") || "(no values)"}`;
}

async function body(criterion: MatchCriterion, table: string): Promise<string> {
  switch (criterion.operator) {
    case "equals":
    case "in":
      return listText(criterion, table);
    case "range": {
      const min = await findColumn(table, criterion.minColumn);
      const max = await columnText(table, criterion.maxColumn);
      return `${min?.label ?? criterion.minColumn} ≤ ${await valueText(criterion.value, min)} < ${max}`;
    }
    default:
      return `${await columnText(table, criterion.column)} is empty`;
  }
}

/** The criterion as one line; lookupTable is the table the criterion's columns belong to. */
export async function criterionText(criterion: MatchCriterion, lookupTable: string): Promise<string> {
  const text = await body(criterion, lookupTable);
  return criterion.operator !== "isEmpty" && criterion.emptyMatchesAll ? `${text} (empty matches all)` : text;
}
