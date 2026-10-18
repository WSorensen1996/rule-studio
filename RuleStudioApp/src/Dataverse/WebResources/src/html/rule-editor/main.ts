import { renderActions } from "./actionsView";
import { loadCatalog } from "./catalog";
import { renderConditions } from "./conditionsView";
import { bindDraftCommands, renderDraftCommands, resetView, viewingLive } from "./draftCommands";
import { bindForm, fillForm } from "./formView";
import { renderHeader, renderNotice } from "./headerView";
import { renderActivity, renderHistory } from "./historyView";
import { copyAsDraft, newRule, newVersion, noneRemoved } from "./model";
import { renderReadout } from "./readoutView";
import { loadRule, loadVersion, loadVersionList } from "./ruleData";
import { saveState } from "./ruleSave";
import { bindSections, bindTriggerCards, renderSections } from "./sectionsView";
import { applyRole } from "../../shared/auth";
import { goBackAfterSave, setLeaveGuard } from "../../shared/back";
import { BatchError } from "../../shared/batch";
import { byId, h } from "../../shared/dom";
import { navigateToPage, openDialog, pageParams } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { loadTableList } from "../../shared/tableMetadata";
import { hostXrm, initWebApi } from "../../shared/xrm";

import type { Catalog } from "./catalog";
import type { EditorContext } from "./context";
import type { CommandHost } from "./draftCommands";
import type { EditorState } from "./model";
import type { VersionListItem } from "./ruleData";

applyStandard();

const KEY_PATTERN = /^[A-Z0-9_]+(\.[A-Z0-9_]+)*$/;

let state: EditorState;
let catalog: Catalog = { functions: [], lookups: [] };
let versions: VersionListItem[] = [];
let author = false;
let saving = false;
// A new rule starts dirty (it has never been saved); leaving it untouched needs no prompt.
let pristine = false;

const statusLine = (): HTMLElement => byId("status");
const liveNumber = (): number | undefined => versions.find((v) => v.id === state.rule.publishedVersionId)?.versionNumber;
const nextNumber = (): number => Math.max(0, ...versions.map((v) => v.versionNumber)) + 1;

const host: CommandHost = {
  state: () => state,
  setState: (next, list) => { state = next; pristine = false; if (list) versions = list; },
  versions: () => versions,
  liveNumber,
  author: () => author,
  saving: () => saving,
  load: (ruleId) => load(ruleId),
  render: () => render(),
};

function context(): EditorContext {
  return { state, catalog, disabled: !author || viewingLive(), changed: versionChanged };
}

function renderBuilder(): void {
  renderConditions(byId("conditionTree"), context());
  renderActions(byId("actionLists"), context());
}

function renderChrome(): void {
  renderHeader(state, versions);
  renderNotice(state, versions, liveNumber(), viewingLive());
  renderHistory(byId("historyList"), versions, state, { onRestore: (item) => void openRestore(item), author });
  renderReadout(byId("readout"), state, catalog);
  renderSections(state);
  renderCommands();
}

function renderCommands(): void {
  byId<HTMLButtonElement>("save").disabled = !author || saving || viewingLive() || !(state.ruleDirty || state.versionDirty);
  byId<HTMLButtonElement>("cmdSaveClose").disabled = !author || saving;
  const createDraft = byId<HTMLButtonElement>("createDraft");
  createDraft.disabled = !author || !state.rule.publishedVersionId;
  if (author) createDraft.title = state.rule.publishedVersionId ? "" : "The rule has no live version yet.";
  renderPublish();
  renderDraftCommands(host);
}

function renderPublish(): void {
  const publish = byId<HTMLButtonElement>("cmdPublish");
  publish.disabled = !author || saving || viewingLive() || state.rule.isNew || !state.rule.draftVersionId || state.ruleDirty || state.versionDirty;
  if (author) publish.title = publishBlocker();
}

// Why Publish is disabled for an author, or "".
function publishBlocker(): string {
  if (state.rule.isNew || state.ruleDirty || state.versionDirty) return "Save the draft first.";
  return state.rule.draftVersionId ? "" : "There is no draft to publish. Change the rule to create one.";
}

function versionChanged(structural: boolean): void {
  state.versionDirty = true;
  pristine = false;
  if (structural) renderBuilder();
  renderChrome();
}

function ruleChanged(): void {
  state.ruleDirty = true;
  pristine = false;
  renderChrome();
}

async function buildState(ruleId: string | undefined): Promise<EditorState> {
  const base = { removed: noneRemoved(), replacesDraftId: null, ruleDirty: false, versionDirty: false };
  if (!ruleId) {
    versions = [];
    return { ...base, rule: newRule(), version: newVersion(1), ruleDirty: true, versionDirty: true };
  }
  const [rule, list] = await Promise.all([loadRule(ruleId), loadVersionList(ruleId)]);
  versions = list;
  if (rule.draftVersionId) return { ...base, rule, version: await loadVersion(rule.draftVersionId) };
  if (rule.publishedVersionId) return { ...base, rule, version: copyAsDraft(await loadVersion(rule.publishedVersionId), nextNumber()) };
  return { ...base, rule, version: newVersion(nextNumber()), versionDirty: true };
}

function render(): void {
  fillForm(state);
  byId<HTMLInputElement>("ruleFiltered").disabled ||= !author;
  renderBuilder();
  renderChrome();
  renderActivity(byId("activity"), state.rule.isNew ? null : state.rule.id)
    .catch((error: unknown) => byId("activity").replaceChildren(h("p", { className: "muted t12" }, describeError("load the execution logs", error))));
}

async function load(ruleId: string | undefined): Promise<void> {
  setStatus(statusLine(), "Loading rule…");
  resetView();
  try {
    [catalog, state] = await Promise.all([loadCatalog(), buildState(ruleId)]);
    pristine = state.rule.isNew;
    render();
    setStatus(statusLine(), "");
  } catch (error) {
    setStatus(statusLine(), describeError("load the rule", error), "error");
  }
}

function validationProblem(): string | null {
  const { rule } = state;
  if (!rule.name.trim()) return "Give the rule a name.";
  if (!KEY_PATTERN.test(rule.ruleKey)) return "The rule key may only use A–Z, 0–9 and _ in dot-separated parts, e.g. PRICING.ANALYTICS_REQUIRES_CORE.";
  return null;
}

function dirty(): boolean {
  return !pristine && !viewingLive() && (state.ruleDirty || state.versionDirty);
}

/** Saves the draft; resolves true when it worked (or there was nothing to save). */
async function save(): Promise<boolean> {
  if (!dirty() && !state.rule.isNew) return true;
  const problem = validationProblem();
  if (problem) {
    setStatus(statusLine(), problem, "error");
    return false;
  }
  saving = true;
  renderChrome();
  setStatus(statusLine(), "Saving…");
  try {
    const createdVersion = state.versionDirty && state.version.isNew;
    await saveState(state);
    toast(createdVersion ? `Draft v${state.version.versionNumber} saved. The live version is unchanged.` : "Saved. The live version is unchanged.");
    await load(state.rule.id);
    return true;
  } catch (error) {
    const message = describeError("save the draft", error);
    setStatus(statusLine(), error instanceof BatchError ? `Could not save the draft. Dataverse said: ${error.message}` : message, "error");
    return false;
  } finally {
    saving = false;
    renderChrome();
  }
}

async function confirmReplace(): Promise<boolean> {
  const draft = versions.find((v) => v.id === state.rule.draftVersionId)?.versionNumber;
  const xrm = hostXrm();
  if (!xrm) return false;
  const result = await xrm.Navigation.openConfirmDialog({
    title: "Replace the current draft?",
    text: `Draft v${draft ?? "?"} will be replaced by a copy of live v${liveNumber() ?? "?"} when you save. The old draft stays in the history as Discarded.`,
    confirmButtonLabel: "Copy live",
  });
  return result.confirmed;
}

async function createDraftFromLive(): Promise<void> {
  const liveId = state.rule.publishedVersionId;
  if (!liveId) return;
  if (state.rule.draftVersionId && !(await confirmReplace())) return;
  try {
    state.version = copyAsDraft(await loadVersion(liveId), nextNumber());
    state.removed = noneRemoved();
    state.replacesDraftId = state.rule.draftVersionId;
    state.versionDirty = true;
    render();
    byId("tabBuilder").click();
  } catch (error) {
    setStatus(statusLine(), describeError("copy the live version", error), "error");
  }
}

function bindTabs(): void {
  document.querySelectorAll<HTMLButtonElement>("[data-tab]").forEach((tab) => {
    tab.addEventListener("click", () => {
      document.querySelectorAll<HTMLButtonElement>("[data-tab]").forEach((other) => {
        const selected = other === tab;
        other.setAttribute("aria-selected", String(selected));
        other.classList.toggle("active", selected);
      });
      document.querySelectorAll<HTMLElement>("[data-tab-panel]").forEach((panel) => {
        panel.hidden = panel.dataset.tabPanel !== tab.dataset.tab;
      });
    });
  });
}

async function openPublish(): Promise<void> {
  try {
    await openDialog("publish-release", { ruleIds: state.rule.id });
  } catch (error) {
    setStatus(statusLine(), describeError("open the publish dialog", error), "error");
  }
  await load(state.rule.id);
}

/** Restores a version as a new draft (restore-version dialog), then reloads onto the rule's draft. */
async function openRestore(item: VersionListItem): Promise<void> {
  if (state.ruleDirty || state.versionDirty) {
    setStatus(statusLine(), "Save or discard your changes before restoring a version.", "error");
    return;
  }
  try {
    await openDialog("restore-version", { versionId: item.id });
  } catch (error) {
    setStatus(statusLine(), describeError("open the restore dialog", error), "error");
  }
  await load(state.rule.id);
}

function bindCommands(): void {
  byId("save").addEventListener("click", () => void save());
  byId("cmdPublish").addEventListener("click", () => void openPublish());
  byId("cmdSaveClose").addEventListener("click", () => void goBackAfterSave(save));
  byId("cmdTest").addEventListener("click", () => void navigateToPage("test-trace", state.rule.isNew ? {} : { ruleId: state.rule.id }));
  byId("cmdTimeline").addEventListener("click", () => void navigateToPage("execution-timeline", state.version.targetTable ? { table: state.version.targetTable } : {}));
  byId("cmdHistory").addEventListener("click", () => byId("tabHistory").click());
  byId("createDraft").addEventListener("click", () => void createDraftFromLive());
  bindDraftCommands(host);
  setLeaveGuard({ dirty, save });
}

function fillTableList(): void {
  loadTableList()
    .then((tables) => byId("tableNames").replaceChildren(...tables.map((t) => h("option", { attrs: { value: t.logicalName } }, t.label))))
    .catch((error: unknown) => console.warn("Rule Studio: table list unavailable", error));
}

function start(): void {
  initWebApi();
  author = applyRole();
  bindTabs();
  bindCommands();
  bindForm({ state: () => state, ruleChanged, versionChanged });
  bindSections();
  bindTriggerCards();
  fillTableList();
  const params = pageParams();
  void load(params.mode === "new" ? undefined : params.ruleId);
}

start();
