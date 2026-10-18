// Step 2 of the form: choose one of the environment's Custom APIs.
import { loadCustomApis } from "./customApi";
import { byId, h } from "../../shared/dom";
import { describeError, setStatus } from "../../shared/notify";

import type { CustomApiInfo } from "./customApi";

let apis: CustomApiInfo[] | null = null;
let onPick: (api: CustomApiInfo) => void = () => {};

function show(picker: boolean): void {
  byId("stepRegister").hidden = picker;
  byId("stepPicker").hidden = !picker;
  byId(picker ? "apiPickerSearch" : "customApiLogicalName").focus();
}

function apiButton(api: CustomApiInfo): HTMLButtonElement {
  const button = h("button", { className: "btn wide api-choice", attrs: { type: "button" } },
    h("span", {}, h("strong", {}, api.displayName), h("span", { className: "mono t11 api-unique" }, api.uniqueName)),
    h("span", { attrs: { "aria-hidden": "true" } }, "→"));
  button.addEventListener("click", () => {
    show(false);
    onPick(api);
  });
  return button;
}

function render(): void {
  const text = byId<HTMLInputElement>("apiPickerSearch").value.trim().toLowerCase();
  const shown = (apis ?? []).filter((api) => `${api.displayName} ${api.uniqueName}`.toLowerCase().includes(text));
  byId("apiPickerResults").replaceChildren(...shown.map(apiButton));
  setStatus(byId("apiCount"), `${shown.length} Custom API${shown.length === 1 ? "" : "s"} found`);
  byId("apiEmpty").hidden = shown.length > 0;
}

async function open(): Promise<void> {
  byId<HTMLInputElement>("apiPickerSearch").value = "";
  show(true);
  if (apis) {
    render();
    return;
  }
  const status = byId("apiCount");
  setStatus(status, "Loading Custom APIs…");
  try {
    apis = await loadCustomApis();
    render();
  } catch (error) {
    setStatus(status, describeError("load the Custom APIs", error), "error");
  }
}

export function initPicker(pick: (api: CustomApiInfo) => void): void {
  onPick = pick;
  byId("apiPickerButton").addEventListener("click", () => void open());
  byId("backToRegister").addEventListener("click", () => show(false));
  byId("apiPickerSearch").addEventListener("input", render);
}
