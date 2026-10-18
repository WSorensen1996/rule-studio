// Environments page: exports the published rules as a bundle and checks or applies an
// imported bundle with rs_ImportRules. Apply is only offered after a clean check of the same file
// (any conflict stops the import, no per-rule override).
import { reportRows } from "./report";
import { applyRole } from "../../shared/auth";
import { byId, h, pill } from "../../shared/dom";
import { describeError, setStatus, toast } from "../../shared/notify";
import { exportRules, importRules } from "../../shared/promotionApi";
import { ApiError } from "../../shared/releaseApi";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { ImportReport } from "../../shared/promotionApi";

applyStandard();

const statusLine = byId("status");
const checkButton = byId<HTMLButtonElement>("check");
const applyButton = byId<HTMLButtonElement>("apply");

let author = false;
let bundle = "";
let checked: ImportReport | null = null;
let busy = false;

// Why Apply is disabled, or "" when it can run.
function applyBlocker(): string {
  if (!bundle) return "Choose a bundle and run the check first.";
  if (!checked) return "Run the check first.";
  if (checked.outcome !== "clean") return "The check found conflicts or errors. Resolve them in Git and check again.";
  return checked.rules.some((r) => r.status === "create" || r.status === "update") ? "" : "Nothing to apply: every rule is unchanged or kept.";
}

function updateButtons(): void {
  checkButton.disabled = !author || busy || !bundle;
  if (!author) return;
  checkButton.title = bundle ? "" : "Choose a bundle first.";
  const blocker = applyBlocker();
  applyButton.disabled = busy || blocker !== "";
  applyButton.title = blocker;
}

function failure(action: string, error: unknown): string {
  return error instanceof ApiError ? `Could not ${action}. Dataverse said: ${error.message}` : describeError(action, error);
}

function download(text: string): void {
  const stamp = new Date().toISOString().slice(0, 16).replace(/[:T]/g, "-");
  const link = h("a", { attrs: { href: URL.createObjectURL(new Blob([text], { type: "application/json" })), download: `rule-studio-${stamp}.json` } });
  document.body.append(link);
  link.click();
  link.remove();
}

async function exportAll(): Promise<void> {
  const button = byId<HTMLButtonElement>("export");
  button.disabled = true;
  setStatus(statusLine, "Exporting published rules…");
  try {
    const text = await exportRules();
    const parsed = JSON.parse(text) as { rules?: unknown[]; lookups?: unknown[] };
    byId("exportInfo").textContent = `Last export: ${parsed.rules?.length ?? 0} rule(s), ${parsed.lookups?.length ?? 0} lookup definition(s).`;
    download(text);
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, failure("export the rules", error), "error");
  } finally {
    button.disabled = false;
  }
}

async function chooseFile(input: HTMLInputElement): Promise<void> {
  const file = input.files?.[0];
  checked = null;
  byId("reportCard").hidden = true;
  bundle = file ? await file.text() : "";
  byId("bundleInfo").textContent = file ? `Bundle: ${file.name} (${Math.ceil(file.size / 1024)} KB). Run the check next.` : "No bundle chosen.";
  setStatus(statusLine, "");
  updateButtons();
}

function showReport(report: ImportReport): void {
  byId("reportCard").hidden = false;
  byId("reportTitle").textContent = report.mode === "apply" ? "Import result" : "Check result";
  const kind = report.outcome === "blocked" ? "bad" : "live";
  const label = { clean: "Clean: ready to apply", blocked: "Blocked: nothing changed", applied: "Applied" }[report.outcome];
  byId("reportOutcome").replaceChildren(pill(label, kind));
  const rows = reportRows(report);
  byId("issues").replaceChildren(rows.issues);
  byId("ruleRows").replaceChildren(...rows.rules);
  byId("lookupRows").replaceChildren(rows.lookups);
  byId("diff").replaceChildren(rows.diff);
}

async function run(mode: "check" | "apply"): Promise<void> {
  busy = true;
  updateButtons();
  setStatus(statusLine, mode === "check" ? "Checking the bundle against this environment…" : "Importing and publishing…");
  try {
    const report = await importRules(bundle, mode);
    showReport(report);
    checked = mode === "check" ? report : null;
    if (report.outcome === "applied") byId("bundleInfo").textContent = "Bundle applied. Run the check again to confirm nothing is left to import.";
    if (report.outcome === "applied") toast(`Imported ${report.rules.filter((r) => r.status === "create" || r.status === "update").length} rule(s).`);
    setStatus(statusLine, "");
  } catch (error) {
    checked = null;
    setStatus(statusLine, failure(mode === "check" ? "check the bundle" : "apply the import", error), "error");
  } finally {
    busy = false;
    updateButtons();
  }
}

function start(): void {
  initWebApi();
  author = applyRole();
  const file = byId<HTMLInputElement>("bundleFile");
  byId("export").addEventListener("click", () => void exportAll());
  file.addEventListener("change", () => void chooseFile(file));
  checkButton.addEventListener("click", () => void run("check"));
  applyButton.addEventListener("click", () => void run("apply"));
  updateButtons();
}

start();
