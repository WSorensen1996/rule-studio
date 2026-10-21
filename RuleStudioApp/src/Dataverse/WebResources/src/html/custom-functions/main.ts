import { loadFunctions, loadUsage } from "./functionData";
import { functionDetail } from "./functionView";
import { applyRole } from "../../shared/auth";
import { byId, fillSelect, h } from "../../shared/dom";
import { openRecord } from "../../shared/nav";
import { describeError, setStatus } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";
import { setFunctionEnabled } from "../register-function/functionRecord";

import type { FunctionRecord, Usage } from "./functionData";

applyStandard();

let functions: FunctionRecord[] = [];
let usage = new Map<string, Usage>();
let selectedId = "";
let author = false;

function render(): void {
  byId("functionCount").textContent = `${functions.length} registered`;
  const picker = byId<HTMLSelectElement>("functionPick");
  const detail = byId("functionDetail");
  const empty = byId("emptyState");
  empty.hidden = functions.length > 0;
  picker.disabled = functions.length === 0;
  if (functions.length === 0) {
    renderActivate();
    picker.replaceChildren();
    empty.textContent = "No functions registered yet. Register one with + Register function.";
    detail.replaceChildren(h("div", { className: "card pad muted" }, "Function details appear here once a function is registered."));
    return;
  }
  if (!functions.some((fn) => fn.id === selectedId)) selectedId = functions[0]?.id ?? "";
  fillSelect(picker, functions.map((fn) => ({ value: fn.id, label: `${fn.name}${fn.enabled ? "" : " (inactive)"}` })));
  picker.value = selectedId;
  const selected = functions.find((fn) => fn.id === selectedId);
  renderActivate(selected);
  if (!selected) return;
  detail.replaceChildren(functionDetail(selected, usage.get(selected.id), {
    author,
    onEdit: () => void openRegister({ functionId: selected.id }),
  }));
}

function renderActivate(selected?: FunctionRecord): void {
  const button = byId<HTMLButtonElement>("cmdActivate");
  button.textContent = selected?.enabled === false ? "▶ Activate" : "⏸ Deactivate";
  button.disabled = !author || !selected;
}

async function toggleActive(): Promise<void> {
  const selected = functions.find((fn) => fn.id === selectedId);
  if (!selected) return;
  const button = byId<HTMLButtonElement>("cmdActivate");
  button.disabled = true;
  try {
    await setFunctionEnabled(selected.id, !selected.enabled);
    selected.enabled = !selected.enabled;
    setStatus(byId("status"), `${selected.name} is now ${selected.enabled ? "active" : "inactive"}.`);
  } catch (error) {
    setStatus(byId("status"), describeError(selected.enabled ? "deactivate the function" : "activate the function", error), "error");
  }
  render();
}

async function refresh(): Promise<void> {
  const status = byId("status");
  const button = byId<HTMLButtonElement>("refresh");
  button.disabled = true;
  setStatus(status, "Loading functions…");
  try {
    functions = await loadFunctions();
    render();
    setStatus(status, "");
    usage = await loadUsage();
    render();
  } catch (error) {
    setStatus(status, describeError("load the functions", error), "error");
  } finally {
    button.disabled = false;
  }
}

async function openRegister(params?: Record<string, string>): Promise<void> {
  try {
    await openRecord("register-function", params);
  } catch (error) {
    setStatus(byId("status"), describeError("open the function form", error), "error");
  }
}

function start(): void {
  initWebApi();
  author = applyRole();
  byId("refresh").addEventListener("click", () => void refresh());
  byId("functionPick").addEventListener("change", (event) => {
    selectedId = (event.target as HTMLSelectElement).value;
    render();
  });
  byId("registerFunction").addEventListener("click", () => void openRegister());
  byId("cmdActivate").addEventListener("click", () => void toggleActive());
  void refresh();
}

start();
