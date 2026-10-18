// Stub of src/shared/tableMetadata: tables come from globalThis.__tables.
// { quote: { label: 'Quote', columns: [{ logicalName, label, kind, options, targets }] } }
const tables = () => globalThis.__tables ?? {};

export async function loadTable(logicalName) {
    const table = tables()[logicalName];
    if (!table) throw new Error(`No table ${logicalName}`);
    return { logicalName, label: table.label, columns: table.columns, children: table.children ?? [] };
}

export async function loadTableList() {
    return Object.entries(tables()).map(([logicalName, t]) => ({ logicalName, label: t.label }));
}

export async function tableLabel(logicalName) {
    return tables()[logicalName]?.label ?? logicalName;
}

export async function findColumn(table, column) {
    return tables()[table]?.columns.find((c) => c.logicalName === column);
}
