// Field paths (json-formats.md, Field path): parsing, storing and showing them to authors.
import { findColumn, loadTable, tableLabel } from "../../shared/tableMetadata";

import type { FieldPath } from "../../shared/formats";
import type { ColumnInfo } from "../../shared/tableMetadata";

function isFieldPath(value: unknown): value is FieldPath {
  const path = value as FieldPath | null;
  return !!path && typeof path.table === "string" && typeof path.column === "string" && Array.isArray(path.hops);
}

/** Reads rs_fieldpath / rs_valuepath. Unknown or broken JSON reads as no path. */
export function parsePath(text: string | null | undefined): FieldPath | null {
  if (!text) return null;
  try {
    const value: unknown = JSON.parse(text);
    return isFieldPath(value) ? value : null;
  } catch {
    return null;
  }
}

export function storePath(path: FieldPath | null): string | null {
  return path ? JSON.stringify({ table: path.table, hops: path.hops, column: path.column }) : null;
}

export function hasChildHop(path: FieldPath | null): boolean {
  return !!path?.hops.some((hop) => hop.kind === "child");
}

/** Table the path's column belongs to: the last hop's table, or the start table. */
export function endTable(path: FieldPath): string {
  return path.hops.at(-1)?.table ?? path.table;
}

export async function columnOf(path: FieldPath | null): Promise<ColumnInfo | undefined> {
  if (!path) return;
  return findColumn(endTable(path), path.column);
}

async function columnLabel(table: string, column: string): Promise<string> {
  try {
    return (await loadTable(table)).columns.find((c) => c.logicalName === column)?.label ?? column;
  } catch {
    return column;
  }
}

/** "Quote → Customer (Account) → Segment", or "Quote ⇉ Quote lines → Product" for a child hop. */
export async function describePath(path: FieldPath): Promise<string> {
  const parts = [await tableLabel(path.table)];
  let table = path.table;
  for (const hop of path.hops) {
    if (hop.kind === "parent") {
      parts.push(`→ ${await columnLabel(table, hop.attribute)} (${await tableLabel(hop.table)})`);
    } else {
      parts.push(`⇉ ${await tableLabel(hop.table)}`);
    }
    table = hop.table;
  }
  parts.push(`→ ${await columnLabel(table, path.column)}`);
  return parts.join(" ");
}

