// Editable drafts of match criteria (json-formats.md §1) and the conversion to and from the
// stored document. Values may only be fixed, a field path or an expression.
import { FORMAT_VERSION } from "../../shared/formats";
import { decodeFixed } from "../rule-editor/fixedValue";

import type { CriterionValueSource, FieldPath, MatchCriteria, MatchCriterion } from "../../shared/formats";

export type Operator = MatchCriterion["operator"];
export type SourceKind = CriterionValueSource["type"];

export interface ValueDraft {
  source: SourceKind;
  /** Fixed value as JSON (the fixed-value input's format). */
  raw: string;
  path: FieldPath | null;
  expression: string;
}

export interface CriterionDraft {
  operator: Operator;
  column: string;
  minColumn: string;
  maxColumn: string;
  value: ValueDraft;
  values: ValueDraft[];
  emptyMatchesAll: boolean;
}

export const OPERATORS: { value: Operator; label: string }[] = [
  { value: "equals", label: "equals" },
  { value: "in", label: "is one of" },
  { value: "range", label: "in range (min ≤ value < max)" },
  { value: "isEmpty", label: "is empty" },
];

export function emptyValue(): ValueDraft {
  return { source: "fieldPath", raw: "", path: null, expression: "" };
}

export function newCriterion(): CriterionDraft {
  return { operator: "equals", column: "", minColumn: "", maxColumn: "", value: emptyValue(), values: [emptyValue()], emptyMatchesAll: false };
}

function toValueDraft(source: CriterionValueSource): ValueDraft {
  const draft = emptyValue();
  draft.source = source.type;
  if (source.type === "fixed") draft.raw = JSON.stringify(source.value);
  else if (source.type === "fieldPath") draft.path = source.path;
  else draft.expression = source.expression;
  return draft;
}

export function toDraft(criterion: MatchCriterion): CriterionDraft {
  const draft = newCriterion();
  draft.operator = criterion.operator;
  if (criterion.operator === "range") {
    draft.minColumn = criterion.minColumn;
    draft.maxColumn = criterion.maxColumn;
  } else {
    draft.column = criterion.column;
  }
  if (criterion.operator === "equals" || criterion.operator === "range") draft.value = toValueDraft(criterion.value);
  if (criterion.operator === "in") draft.values = criterion.values.map(toValueDraft);
  if (criterion.operator !== "isEmpty") draft.emptyMatchesAll = criterion.emptyMatchesAll === true;
  return draft;
}

/** Thrown with an author-facing message when a draft cannot be stored. */
export class DraftError extends Error {}

function toSource(draft: ValueDraft, recordTable: string, where: string): CriterionValueSource {
  if (draft.source === "fieldPath") {
    if (!draft.path) throw new DraftError(`${where}: choose the record field to compare with.`);
    if (draft.path.table !== recordTable) throw new DraftError(`${where}: the field path must start at the record table (${recordTable}).`);
    if (draft.path.hops.some((hop) => hop.kind !== "parent")) throw new DraftError(`${where}: only related (parent) records can be used.`);
    return { type: "fieldPath", path: draft.path };
  }
  if (draft.source === "expression") {
    if (!draft.expression.trim()) throw new DraftError(`${where}: enter an expression.`);
    return { type: "expression", expression: draft.expression.trim() };
  }
  return { type: "fixed", value: decodeFixed(draft.raw)[0] ?? null };
}

function need(column: string, where: string): string {
  if (!column) throw new DraftError(`${where}: choose a lookup table column.`);
  return column;
}

function toCriterion(draft: CriterionDraft, recordTable: string, index: number): MatchCriterion {
  const where = `Criterion ${index + 1}`;
  const wildcard = draft.emptyMatchesAll ? { emptyMatchesAll: true } : {};
  switch (draft.operator) {
    case "equals":
      return { operator: "equals", column: need(draft.column, where), value: toSource(draft.value, recordTable, where), ...wildcard };
    case "in":
      if (draft.values.length === 0) throw new DraftError(`${where}: add at least one value.`);
      return { operator: "in", column: need(draft.column, where), values: draft.values.map((v) => toSource(v, recordTable, where)), ...wildcard };
    case "range":
      return {
        operator: "range", minColumn: need(draft.minColumn, where), maxColumn: need(draft.maxColumn, where),
        value: toSource(draft.value, recordTable, where), ...wildcard,
      };
    default:
      return { operator: "isEmpty", column: need(draft.column, where) };
  }
}

/** The rs_matchcriteria document. Throws DraftError when a criterion is incomplete. */
export function toDocument(recordTable: string, drafts: CriterionDraft[]): MatchCriteria {
  return { version: FORMAT_VERSION, recordTable, criteria: drafts.map((d, i) => toCriterion(d, recordTable, i)) };
}
