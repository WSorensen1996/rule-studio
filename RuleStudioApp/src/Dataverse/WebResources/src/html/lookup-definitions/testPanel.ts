// "Test lookup": evaluates the selected lookup definition against a real or sample record with
// rs_SimulateRule (LookupDefinitionId). Nothing is saved.
import { h, pill } from "../../shared/dom";
import { describeError } from "../../shared/notify";
import { ApiError, testLookup } from "../../shared/releaseApi";
import { formatValue } from "../../shared/traceView";

import type { LookupSummary } from "./lookupData";
import type { LookupTrace } from "../../shared/formats";
import type { LookupTestResult } from "../../shared/releaseApi";

const POLICY_TEXT: Record<string, string> = {
  usedDefault: "No row matched: the default value was used.",
  skipped: "No row matched: the condition or action would be skipped.",
  blocked: "No row matched: the save would be blocked.",
  error: "Several rows matched and the lookup allows only one.",
};

function parseSamples(text: string): Record<string, unknown> | undefined {
  if (!text.trim()) return undefined;
  const value: unknown = JSON.parse(text);
  if (typeof value !== "object" || value === null || Array.isArray(value)) throw new Error("Sample values must be a JSON object, for example {\"name\": \"Contoso\"}.");
  return value as Record<string, unknown>;
}

function lookupLines(lookup: LookupTrace): HTMLElement[] {
  const lines: HTMLElement[] = [];
  if (lookup.value !== undefined) lines.push(h("div", {}, "Value: ", h("b", { className: "mono" }, formatValue(lookup.value))));
  if (lookup.matchedRowId) lines.push(h("div", { className: "muted t11 mono" }, `Matched row ${lookup.matchedRowId} (${lookup.wildcards} wildcard(s))`));
  if (lookup.policy) lines.push(h("div", { className: "muted t11" }, POLICY_TEXT[lookup.policy] ?? lookup.policy));
  return lines;
}

function resultView(result: LookupTestResult): HTMLElement {
  const head = h("div", {}, result.error ? pill("Failed", "bad") : pill("Resolved", "live"), ` ${result.lookup?.matchCount ?? 0} matching row(s)`);
  const error = result.error ? [h("div", { className: "t11" }, `${result.error.code}: ${result.error.message}`)] : [];
  return h("div", { className: "scope-box section-gap" }, head, ...(result.lookup ? lookupLines(result.lookup) : []), ...error);
}

/** The test form; the record table comes from the lookup's match criteria. */
export function testPanel(lookup: LookupSummary): HTMLElement {
  const table = lookup.criteria?.recordTable ?? "";
  const recordId = h("input", { className: "input mono", attrs: { type: "text", placeholder: "Record id (optional)", "aria-label": "Record id" } });
  const samples = h("textarea", { className: "textarea mono", attrs: { rows: "3", placeholder: "Sample values as JSON, e.g. {\"address1_country\": \"Denmark\"}", "aria-label": "Sample values" } });
  const run = h("button", { className: "btn primary", attrs: { type: "button" } }, "Run test");
  const output = h("div", { attrs: { role: "status", "aria-live": "polite" } });
  run.disabled = !table;
  if (!table) run.title = "The lookup has no match criteria with a record table.";
  run.addEventListener("click", () => {
    run.disabled = true;
    output.replaceChildren(h("p", { className: "muted t12" }, "Evaluating…"));
    void (async () => {
      try {
        const result = await testLookup(lookup.id, table, recordId.value.trim() || undefined, parseSamples(samples.value));
        output.replaceChildren(resultView(result));
      } catch (error) {
        const text = error instanceof ApiError ? `Dataverse said: ${error.message}` : describeError("test the lookup", error);
        output.replaceChildren(h("p", { className: "status-line error" }, text));
      } finally {
        run.disabled = false;
      }
    })();
  });
  return h("div", { className: "card pad section-gap" },
    h("h3", {}, "⚗ Test lookup"),
    h("p", { className: "muted t11" }, `Evaluated for a record of ${table || "?"}. Nothing is saved.`),
    recordId, samples, h("div", { className: "detail-actions" }, run), output);
}
