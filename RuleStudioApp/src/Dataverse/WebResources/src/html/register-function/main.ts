import { initPicker } from "./apiPicker";
import { loadApiShape } from "./customApi";
import { loadFunction, saveFunction } from "./functionRecord";
import { ParamGrid, TYPE_OPTIONS } from "./paramGrid";
import { applyRole } from "../../shared/auth";
import { goBackAfterSave, setLeaveGuard } from "../../shared/back";
import { byId, fillSelect, h, pill } from "../../shared/dom";
import { pageParams } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { loadTableList } from "../../shared/tableMetadata";
import { initWebApi } from "../../shared/xrm";

import type { CustomApiInfo } from "./customApi";
import type { FunctionForm } from "./functionRecord";
import type { ParameterType } from "../../shared/formats";

applyStandard();

let functionId = pageParams().functionId ?? "";
const status = byId("status");
const name = byId<HTMLInputElement>("funcName");
const apiName = byId<HTMLInputElement>("customApiLogicalName");
const output = byId<HTMLSelectElement>("funcOutput");
const timeout = byId<HTMLInputElement>("funcTimeout");
const description = byId<HTMLTextAreaElement>("funcDescription");
// Active / inactive is set from the Custom functions header; a new function starts active.
let enabled = true;
const sideEffects = byId<HTMLInputElement>("funcSideEffects");
const safeRetry = byId<HTMLInputElement>("funcSafeRetry");
let grid: ParamGrid;
// The form as last loaded or saved; Back asks first when the form differs from it.
let saved = "";

function showNotice(title: string, text: string): void {
  const notice = byId("apiNotice");
  notice.replaceChildren(h("strong", {}, title), text);
  notice.hidden = false;
}

function fillForm(form: FunctionForm): void {
  name.value = form.name;
  apiName.value = form.apiName;
  description.value = form.description;
  output.value = form.output;
  timeout.value = form.timeoutMs === null ? "" : String(form.timeoutMs);
  enabled = form.enabled;
  sideEffects.checked = form.hasSideEffects;
  safeRetry.checked = form.safeToRetry;
  grid.set(form.parameters);
  showTitle(form.name, form.enabled);
  if (form.schemaUnreadable) {
    showNotice("⚠ Stored input schema unreadable", "Define the parameters again; saving replaces the stored schema.");
  }
}

function readForm(): FunctionForm {
  const ms = Number.parseInt(timeout.value, 10);
  return {
    name: name.value.trim(),
    apiName: apiName.value.trim(),
    description: description.value.trim(),
    enabled,
    hasSideEffects: sideEffects.checked,
    safeToRetry: safeRetry.checked,
    timeoutMs: Number.isFinite(ms) && ms > 0 ? ms : null,
    output: output.value as ParameterType,
    parameters: grid.value(),
    schemaUnreadable: false,
  };
}

function showTitle(title: string, active: boolean): void {
  byId("pageTitle").textContent = title || "Custom function";
  byId("fnStatus").replaceChildren(active ? pill("Active", "live") : pill("Inactive", "muted"));
  document.title = `${title || "Custom function"} · Rule Studio`;
}

const snapshot = (): string => JSON.stringify(readForm());

function problem(form: FunctionForm): string | null {
  if (!form.name) return "Enter a display name.";
  if (!form.apiName) return "Enter the Custom API logical name, or pick one with the search button.";
  if (timeout.value && form.timeoutMs === null) return "Timeout must be a whole number of milliseconds above zero.";
  return grid.problem();
}

/** Saves the form; resolves true when it worked. */
async function save(): Promise<boolean> {
  const form = readForm();
  const issue = problem(form);
  if (issue) {
    setStatus(status, issue, "error");
    return false;
  }
  const buttons = [byId<HTMLButtonElement>("save"), byId<HTMLButtonElement>("cmdSaveClose")];
  for (const button of buttons) button.disabled = true;
  setStatus(status, "Saving…");
  try {
    const created = !functionId;
    functionId = await saveFunction(functionId, form);
    toast(created ? "Function registered." : "Function registration updated.");
    saved = snapshot();
    showTitle(form.name, form.enabled);
    setStatus(status, "");
    return true;
  } catch (error) {
    const detail = error instanceof Error ? ` ${error.message}` : "";
    setStatus(status, `${describeError("save the function", error)}${detail}`, "error");
    return false;
  } finally {
    for (const button of buttons) button.disabled = false;
  }
}

async function pickApi(api: CustomApiInfo): Promise<void> {
  apiName.value = api.uniqueName;
  if (!name.value.trim()) name.value = api.displayName;
  if (!description.value.trim()) description.value = api.description;
  setStatus(status, "Reading the Custom API parameters…");
  try {
    const shape = await loadApiShape(api.id);
    grid.set(shape.parameters);
    if (shape.output) output.value = shape.output;
    byId("apiNotice").hidden = shape.unsupported.length === 0;
    if (shape.unsupported.length) {
      showNotice("⚠ Unsupported parameter types", `Rule Studio cannot map these request parameters, so they are left out: ${shape.unsupported.join(", ")}.`);
    }
    setStatus(status, "");
  } catch (error) {
    setStatus(status, describeError("read the Custom API parameters", error), "error");
  }
}

async function start(): Promise<void> {
  initWebApi();
  const author = applyRole();
  fillSelect(output, TYPE_OPTIONS);
  grid = new ParamGrid(byId("paramRows"), author);
  grid.set([]);
  initPicker((api) => void pickApi(api));
  byId("addParameter").addEventListener("click", () => grid.add());
  byId("save").addEventListener("click", () => void save());
  byId("cmdSaveClose").addEventListener("click", () => void goBackAfterSave(save));
  setLeaveGuard({ dirty: () => author && snapshot() !== saved, save });
  void loadTableList().then((tables) => grid.setTables(tables), (error: unknown) => {
    setStatus(status, describeError("load the table list", error), "error");
  });
  saved = snapshot();
  if (!functionId) return;
  setStatus(status, "Loading function…");
  try {
    fillForm(await loadFunction(functionId));
    saved = snapshot();
    setStatus(status, "");
  } catch (error) {
    setStatus(status, describeError("load the function", error), "error");
    byId<HTMLButtonElement>("save").disabled = true;
    byId<HTMLButtonElement>("cmdSaveClose").disabled = true;
  }
}

void start();
