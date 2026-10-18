// The function list cards and the detail panel of the selected function.
import { outputLabel } from "./functionData";
import { h, pill } from "../../shared/dom";

import type { FunctionRecord, Usage } from "./functionData";
import type { FunctionParameter, ParameterType } from "../../shared/formats";

const TYPE_LABELS: Record<ParameterType, string> = {
  text: "Text", number: "Number", decimal: "Decimal", money: "Money", boolean: "Boolean", date: "Date", lookup: "Lookup",
};

function behaviourPills(fn: FunctionRecord): HTMLElement {
  return h("div", { className: "card-pills" },
    fn.hasSideEffects ? pill("Side effects", "warning") : pill("Pure", "live"), " ",
    fn.safeToRetry ? pill("Retry-safe", "info") : pill("One attempt", "warning"),
    fn.enabled ? null : h("span", {}, " ", pill("Inactive", "muted")));
}

export function functionCard(fn: FunctionRecord, selected: boolean, onSelect: () => void): HTMLButtonElement {
  const card = h("button", { className: "select-card", attrs: { type: "button", "aria-pressed": String(selected) } },
    h("b", { className: "t12" }, fn.name),
    h("div", { className: "muted t11 mono" }, fn.apiName || "No Custom API name"),
    behaviourPills(fn));
  card.addEventListener("click", onSelect);
  return card;
}

function plural(count: number, word: string): string {
  return `${count} ${word}${count === 1 ? "" : "s"}`;
}

export function usageText(usage: Usage | undefined): string {
  if (!usage || usage.versions === 0) return "Not used by any rule.";
  return `Used by ${plural(usage.rules, "rule")} (${plural(usage.versions, "version")}). It cannot be deleted while in use.`;
}

function parameterRow(parameter: FunctionParameter): HTMLTableRowElement {
  return h("tr", {},
    h("td", {}, h("span", { className: "mono" }, parameter.name),
      parameter.displayName ? h("div", { className: "muted t11" }, parameter.displayName) : null),
    h("td", {}, TYPE_LABELS[parameter.type] ?? parameter.type),
    h("td", {}, parameter.required ? "Yes" : "No"),
    h("td", { className: parameter.table ? "mono" : "muted" }, parameter.table ?? "—"),
    h("td", { className: "muted" }, parameter.description ?? ""));
}

function parameterSection(fn: FunctionRecord): HTMLElement {
  if (fn.parameters === null) {
    return h("div", { className: "notice" }, h("strong", {}, "⚠ Input schema missing or unreadable"),
      "Edit the function to define its parameters before rules can map inputs.");
  }
  if (fn.parameters.length === 0) return h("p", { className: "muted t12" }, "This function takes no inputs.");
  return h("div", { className: "table-scroll" }, h("table", { className: "data-table" },
    h("thead", {}, h("tr", {}, ...["Parameter", "Type", "Required", "Table", "Description"].map((t) => h("th", {}, t)))),
    h("tbody", {}, ...fn.parameters.map(parameterRow))));
}

function contractBox(): HTMLElement {
  return h("div", { className: "scope-box mono" }, "{",
    h("div", { className: "contract-body" }, "\"success\": true,", h("br"), "\"value\": …,", h("br"), "\"message\": \"Completed\""), "}");
}

function notice(ok: boolean, title: string, text: string): HTMLElement {
  return h("div", { className: ok ? "notice ok" : "notice" }, h("strong", {}, title), text);
}

function behaviourNotices(fn: FunctionRecord): HTMLElement {
  const effects = fn.hasSideEffects
    ? notice(false, "⚠ Side effects", "Tests must supply or use a mocked result; the actual API is not invoked.")
    : notice(true, "✓ Side-effect free", "Test & trace calls it for real when “Mock all functions” is off.");
  const retry = fn.safeToRetry
    ? notice(true, "✓ Safe to retry", "Retries may follow the rule retry policy.")
    : notice(false, "⚠ Not safe to retry", "Attempt only once even if the rule requests retries.");
  return h("div", { className: "grid g2 section-gap" }, effects, retry);
}

function fieldGrid(fn: FunctionRecord): HTMLElement {
  return h("div", { className: "field-grid" },
    h("div", {}, h("div", { className: "form-label" }, "Custom API logical name"), h("code", {}, fn.apiName || "—")),
    h("div", {}, h("div", { className: "form-label" }, "Output type"), pill(outputLabel(fn.outputType), "info")),
    h("div", {}, h("div", { className: "form-label" }, "Timeout"), fn.timeoutMs ? `${fn.timeoutMs} ms` : "Default"));
}

function heading(fn: FunctionRecord): HTMLElement {
  return h("div", { className: "detail-heading" },
    h("div", { className: "rs-summary" },
      h("div", { className: "rs-summary-pills" }, fn.enabled ? pill("Active", "live") : pill("Inactive", "muted")),
      h("p", { className: "rs-summary-text" }, "Custom function")),
    h("div", { className: "rs-title" }, h("h2", {}, fn.name), h("p", { className: "rs-desc" }, fn.description || "No description.")));
}

export interface DetailHandlers {
  author: boolean;
  onEdit: () => void;
}

export function functionDetail(fn: FunctionRecord, usage: Usage | undefined, handlers: DetailHandlers): HTMLElement {
  const edit = h("button", { className: "btn mini", attrs: { type: "button" } }, "Edit registration");
  edit.disabled = !handlers.author;
  if (!handlers.author) edit.title = "Requires the Rule Studio Author role.";
  edit.addEventListener("click", handlers.onEdit);
  return h("div", { className: "card pad" },
    heading(fn),
    h("div", { className: "divider" }),
    fieldGrid(fn),
    h("h3", { className: "detail-heading" }, "Typed inputs"),
    parameterSection(fn),
    h("h3", { className: "detail-heading" }, "Uniform result contract"),
    contractBox(),
    behaviourNotices(fn),
    h("div", { className: "divider" }),
    h("p", { className: "muted t12" }, usageText(usage)),
    h("p", { className: "muted t12" }, "Functions are created in code and registered here as metadata. A form-based author selects them as an action or value source and maps input parameters; the editor does not compile new plug-in code."),
    edit);
}
