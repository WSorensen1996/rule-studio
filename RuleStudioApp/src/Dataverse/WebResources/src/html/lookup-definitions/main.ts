import { renderDetail } from "./detailView";
import { confirmDeleteLookup, deleteLookup, loadLookups } from "./lookupData";
import { testPanel } from "./testPanel";
import { applyRole } from "../../shared/auth";
import { byId, fillSelect } from "../../shared/dom";
import { openRecord, pageParams } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { LookupSummary } from "./lookupData";

applyStandard();

const picker = byId<HTMLSelectElement>("lookupPick");
const statusLine = (): HTMLElement => byId("status");

let lookups: LookupSummary[] = [];
let selectedId = "";
let author = false;
let testing = false;

function render(): void {
  fillSelect(picker, lookups.map((l) => ({ value: l.id, label: `${l.name} · ${l.lookupTable || "no table"}` })));
  picker.value = selectedId;
  picker.disabled = lookups.length === 0;
  const empty = byId("emptyState");
  empty.hidden = lookups.length > 0;
  empty.textContent = "No lookup definitions yet. Create the first one with + Define lookup.";
  const selected = selectedLookup();
  updateCommands(selected);
  if (!selected) {
    byId("detail").replaceChildren();
    return;
  }
  byId("detail").replaceChildren(renderDetail(selected), ...(testing ? [testPanel(selected)] : []));
}

function selectedLookup(): LookupSummary | undefined {
  return lookups.find((l) => l.id === selectedId);
}

const AUTHOR_ONLY = "Requires the Rule Studio Author role.";

/** Edit, Test and Delete in the command bar act on the selected lookup. */
function updateCommands(selected: LookupSummary | undefined): void {
  const edit = byId<HTMLButtonElement>("cmdEdit");
  const remove = byId<HTMLButtonElement>("cmdDelete");
  byId<HTMLButtonElement>("cmdTest").disabled = !selected;
  edit.disabled = !selected || !author;
  edit.title = selected && !author ? AUTHOR_ONLY : "";
  remove.disabled = !selected || !author || selected.usage > 0;
  if (selected && selected.usage > 0) remove.title = "Used by conditions or actions. Remove those references first.";
  else remove.title = selected && !author ? AUTHOR_ONLY : "";
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

async function remove(lookup: LookupSummary): Promise<void> {
  try {
    if (!(await confirmDeleteLookup(lookup.name))) return;
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
  picker.addEventListener("change", () => {
    selectedId = picker.value;
    testing = false;
    render();
  });
  byId("refresh").addEventListener("click", () => void refresh());
  byId("newLookup").addEventListener("click", () => void openForm());
  byId("cmdEdit").addEventListener("click", () => {
    const lookup = selectedLookup();
    if (lookup) void openForm({ lookupId: lookup.id });
  });
  byId("cmdTest").addEventListener("click", showTest);
  byId("cmdDelete").addEventListener("click", () => {
    const lookup = selectedLookup();
    if (lookup) void remove(lookup);
  });
  void refresh();
}

start();
