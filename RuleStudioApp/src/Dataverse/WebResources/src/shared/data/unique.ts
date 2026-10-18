// Dataverse returns a collection-valued $expand of a list query inline and also with an
// @odata.nextLink; XrmQuery follows the link and appends the same rows again. Expanded rows are
// therefore de-duplicated by their primary key wherever a list query expands a collection.
export function uniqueBy<T>(rows: readonly T[] | null | undefined, key: (row: T) => string): T[] {
  const seen = new Set<string>();
  return (rows ?? []).filter((row) => {
    const id = key(row);
    if (seen.has(id)) return false;
    seen.add(id);
    return true;
  });
}
