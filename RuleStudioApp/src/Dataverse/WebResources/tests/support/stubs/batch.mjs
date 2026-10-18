// Stub of src/shared/batch: records the changeset instead of sending it.
export class BatchError extends Error {}

export function bindTo(entitySet, id) {
    return id ? `/${entitySet}(${id})` : null;
}

export async function executeChangeset(requests) {
    globalThis.__changesets = [...(globalThis.__changesets ?? []), requests];
}
