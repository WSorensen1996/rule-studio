import { renderDetail } from "./detailView";
import { deleteLookup, loadLookups } from "./lookupData";
import { testPanel } from "./testPanel";
import { applyRole } from "../../shared/auth";
import { byId, h } from "../../shared/dom";
import { openRecord, pageParams } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { hostXrm, initWebApi } from "../../shared/xrm";

import type { LookupSummary } from "./lookupData";

applyStandard();

const search = byId<HTMLInputElement>("lookupSearch");
const statusLine = (): HTMLElement => byId("status");

let lookups: LookupSummary[] = [];
let selectedId = "";
let author = false;
let testing = false;

function matches(lookup: LookupSummary): boolean {
  const text = search.value.trim().toLowerCase();
  return !text || `${lookup.name} ${lookup.lookupTable}`.toLowerCase().includes(text);
}

function listCard(lookup: LookupSummary): HTMLButtonElement {
  const card = h("button", { className: "select-card lookup-card", attrs: { type: "button", "aria-pressed": String(lookup.id === selectedId) } },
    h("b", {}, lookup.name), h("div", { className: "muted t11 mono" }, lookup.lookupTable || "No table set"));
  card.addEventListener("click", () => {
    selectedId = lookup.id;
    testing = false;
    render();
  });
  return card;
}

function render(): void {
  const shown = lookups.filter(matches);
  byId("lookupList").replaceChildren(...shown.map(listCard));
  const empty = byId("emptyState");
  empty.hidden = shown.length > 0;
  empty.textContent = lookups.length === 0
    ? "No lookup definitions yet. Create the first one with + Define lookup."
    : "No lookups match this search.";
  const selected = lookups.find((l) => l.id === selectedId);
  if (!selected) {
    byId("detail").replaceChildren(h("div", { className: "card pad muted" }, lookups.length ? "Select a lookup to see its configuration." : "Nothing to show yet."));
    return;
  }
  const handlers = { author, onEdit: (l: LookupSummary) => void openForm({ lookupId: l.id }), onDelete: (l: LookupSummary) => void remove(l), onTest: showTest };
  byId("detail").replaceChildren(renderDetail(selected, handlers), ...(testing ? [testPanel(selected)] : []));
}

function showTest(): void {
  testing = true;
  render();
}

async function refresh(): Promise<void> {
  const button = byId<HTMLButtonElement>("refresh");
  button.disabled = true;
  setStatus(statusLine(), "Loading lookup definitions…");
  try {
    lookups = await loadLookups();
    if (!lookups.some((l) => l.id === selectedId)) selectedId = lookups[0]?.id ?? "";
    render();
    setStatus(statusLine(), "");
  } catch (error) {
    setStatus(statusLine(), describeError("load the lookup definitions", error), "error");
  } finally {
    button.disabled = false;
  }
}

async function openForm(params?: Record<string, string>): Promise<void> {
  try {
    await openRecord("new-lookup-definition", params);
  } catch (error) {
    setStatus(statusLine(), describeError("open the lookup definition form", error), "error");
  }
}

async function confirmDelete(name: string): Promise<boolean> {
  const xrm = hostXrm();
  if (!xrm) return false;
  const result = await xrm.Navigation.openConfirmDialog({
    title: "Delete lookup definition",
    text: `Delete "${name}"? This cannot be undone.`,
    confirmButtonLabel: "Delete",
  });
  return result.confirmed;
}

async function remove(lookup: LookupSummary): Promise<void> {
  try {
    if (!(await confirmDelete(lookup.name))) return;
    await deleteLookup(lookup.id);
    toast(`Lookup "${lookup.name}" deleted.`);
    await refresh();
  } catch (error) {
    setStatus(statusLine(), describeError("delete the lookup definition (it may be in use)", error), "error");
  }
}

function start(): void {
  initWebApi();
  author = applyRole();
  selectedId = pageParams().lookupId ?? "";
  search.addEventListener("input", render);
  byId("refresh").addEventListener("click", () => void refresh());
  byId("newLookup").addEventListener("click", () => void openForm());
  void refresh();
}

start();
