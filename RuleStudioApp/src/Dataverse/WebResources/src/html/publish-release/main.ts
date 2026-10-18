// Validate & publish release: pick rule drafts, validate them, then publish them as one release.
import { renderResult } from "./resultView";
import { pickCandidates, renderRuleList, selectedIds } from "./ruleList";
import { applyRole } from "../../shared/auth";
import { loadRules } from "../../shared/data/rules";
import { byId, h } from "../../shared/dom";
import { closeDialog, pageParams } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { ApiError, publishRelease, validateRelease } from "../../shared/releaseApi";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { Candidates } from "./ruleList";
import type { ValidationResult } from "../../shared/formats";

applyStandard();

const name = byId<HTMLInputElement>("releaseName");
const description = byId<HTMLTextAreaElement>("releaseDescription");
const validateButton = byId<HTMLButtonElement>("validate");
const publishButton = byId<HTMLButtonElement>("publish");
const validation = byId("validation");
const statusLine = byId("status");
const params = pageParams();

let candidates: Candidates = { rules: [], selected: new Set() };
let author = false;
let busy = false;
/** Selection key of the last validation that did not fail; publishing needs it to match. */
let validatedKey: string | null = null;

const selectionKey = (): string => selectedIds(candidates).map((id) => id.toLowerCase()).sort().join(",");

function failMessage(action: string, error: unknown): string {
  return error instanceof ApiError ? `Could not ${action}. Dataverse said: ${error.message}` : describeError(action, error);
}

function updateButtons(): void {
  const count = candidates.selected.size;
  byId("selectionCount").textContent = `${count} of ${candidates.rules.length} selected`;
  validateButton.disabled = !author || busy || count === 0;
  publishButton.disabled = !author || busy || count === 0 || validatedKey !== selectionKey();
  if (!author) return;
  validateButton.title = count === 0 ? "Select at least one draft." : "";
  if (count === 0) publishButton.title = "Select at least one draft.";
  else publishButton.title = validatedKey === selectionKey() ? "" : "Validate the selection first; publishing is enabled once validation passes.";
}

function selectionChanged(): void {
  if (validatedKey !== null && validatedKey !== selectionKey()) {
    validatedKey = null;
    validation.replaceChildren(h("p", { className: "muted t12" }, "The selection changed. Validate again before publishing."));
  }
  updateButtons();
}

function showResult(result: ValidationResult): void {
  const names = new Map(candidates.rules.map((rule) => [rule.ruleKey, rule.name]));
  renderResult(validation, result, names);
}

function pad(value: number): string {
  return String(value).padStart(2, "0");
}

function defaultName(): string {
  const now = new Date();
  return `Release ${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())} ${pad(now.getHours())}:${pad(now.getMinutes())}`;
}

async function runBusy(text: string, work: () => Promise<void>): Promise<void> {
  busy = true;
  updateButtons();
  setStatus(statusLine, text);
  try {
    await work();
  } finally {
    busy = false;
    updateButtons();
  }
}

async function validate(): Promise<void> {
  const key = selectionKey();
  await runBusy("Validating…", async () => {
    try {
      const result = await validateRelease(selectedIds(candidates));
      validatedKey = result.outcome === "failed" ? null : key;
      showResult(result);
      setStatus(statusLine, "");
    } catch (error) {
      validatedKey = null;
      setStatus(statusLine, failMessage("validate the release", error), "error");
    }
  });
}

async function publish(): Promise<void> {
  const releaseName = name.value.trim();
  if (!releaseName) {
    setStatus(statusLine, "Enter a release name.", "error");
    name.focus();
    return;
  }
  const ruleIds = selectedIds(candidates);
  await runBusy("Publishing…", async () => {
    try {
      const response = await publishRelease({ ruleIds, name: releaseName, description: description.value.trim() || undefined });
      showResult(response.validation);
      if (!response.releaseId) {
        validatedKey = null;
        setStatus(statusLine, "Validation failed when publishing, so nothing was published. Review the issues below.", "error");
        return;
      }
      toast(`Published ${ruleIds.length} rule${ruleIds.length === 1 ? "" : "s"} as release "${releaseName}".`);
      closeDialog();
    } catch (error) {
      setStatus(statusLine, failMessage("publish the release", error), "error");
    }
  });
}

async function load(): Promise<void> {
  setStatus(statusLine, "Loading drafts…");
  try {
    const ruleIds = (params.ruleIds ?? "").split(",").map((id) => id.trim()).filter(Boolean);
    candidates = pickCandidates(await loadRules(), ruleIds);
    renderRuleList(byId("ruleList"), candidates, selectionChanged);
    applyRole(byId("ruleList"));
    name.value = defaultName();
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the drafts", error), "error");
  }
  updateButtons();
}

function start(): void {
  initWebApi();
  author = applyRole();
  updateButtons();
  byId("cancel").addEventListener("click", closeDialog);
  validateButton.addEventListener("click", () => void validate());
  byId("releaseForm").addEventListener("submit", (event) => {
    event.preventDefault();
    if (!publishButton.disabled) void publish();
  });
  void load();
}

start();
