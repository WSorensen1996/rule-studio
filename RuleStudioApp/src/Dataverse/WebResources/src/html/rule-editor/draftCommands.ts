// Toolbar commands around the draft: Discard draft, Delete rule, Copy rule and View live.
import { copyAsDraft, newRule, noneRemoved } from "./model";
import { loadVersion } from "./ruleData";
import { deleteRule, discardDraft } from "./ruleSave";
import { BatchError } from "../../shared/batch";
import { byId } from "../../shared/dom";
import { goBack } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { hostXrm } from "../../shared/xrm";

import type { EditorState } from "./model";
import type { VersionListItem } from "./ruleData";

/** What the commands need from the editor page. */
export interface CommandHost {
  state(): EditorState;
  setState(state: EditorState, versions?: VersionListItem[]): void;
  versions(): VersionListItem[];
  liveNumber(): number | undefined;
  author(): boolean;
  saving(): boolean;
  load(ruleId: string): Promise<void>;
  render(): void;
}

let viewing = false;

/** True while the live version is shown read-only. */
export function viewingLive(): boolean {
  return viewing;
}

/** Called when the editor (re)loads the rule: it always opens on the draft. */
export function resetView(): void {
  viewing = false;
}

const statusLine = (): HTMLElement => byId("status");

function button(id: string, disabled: boolean, hidden = false): HTMLButtonElement {
  const element = byId<HTMLButtonElement>(id);
  element.disabled = disabled;
  element.hidden = hidden;
  return element;
}

/** Discard draft when there is a live version to go back to; otherwise Delete rule (a live rule can't be deleted). */
export function renderDraftCommands(host: CommandHost): void {
  const state = host.state();
  const { rule } = state;
  const locked = !host.author() || host.saving();
  const hasLive = !!rule.publishedVersionId;
  const dirty = state.ruleDirty || state.versionDirty;
  button("cmdDiscard", locked || viewing || !(rule.draftVersionId || dirty), !hasLive);
  button("cmdDelete", locked || rule.isNew, hasLive);
  button("cmdCopy", locked || viewing || rule.isNew).title = viewing ? "Go back to the draft first." : "";
  renderViewLive(host, hasLive);
}

function renderViewLive(host: CommandHost, hasLive: boolean): void {
  const view = button("cmdViewLive", host.saving() || (!viewing && !(hasLive && host.state().rule.draftVersionId)));
  view.textContent = viewing ? "✎ Back to draft" : "👁 View live";
  view.title = view.disabled && hasLive ? "You are already looking at the live version." : "";
}

async function confirm(title: string, text: string, confirmButtonLabel: string): Promise<boolean> {
  const xrm = hostXrm();
  if (!xrm) return false;
  return (await xrm.Navigation.openConfirmDialog({ title, text, confirmButtonLabel })).confirmed;
}

/** Throws away the draft (kept in history as Discarded) and unsaved changes, then shows the live version. */
async function discard(host: CommandHost): Promise<void> {
  const { rule } = host.state();
  const draftId = rule.draftVersionId;
  const draft = host.versions().find((v) => v.id === draftId)?.versionNumber ?? "?";
  const live = host.liveNumber() ?? "?";
  const text = draftId
    ? `Draft v${draft} and any unsaved changes are thrown away and the editor goes back to live v${live}. The draft stays in the history as Discarded.`
    : `Your unsaved changes are thrown away and the editor goes back to live v${live}.`;
  if (!(await confirm("Discard the draft?", text, "Discard"))) return;
  try {
    if (draftId) await discardDraft(rule.id, draftId);
    toast("Draft discarded. Showing the live version.");
    await host.load(rule.id);
  } catch (error) {
    setStatus(statusLine(), describeError("discard the draft", error), "error");
  }
}

/** Deletes a rule that is not live, with its versions, then goes back. */
async function remove(host: CommandHost): Promise<void> {
  const { rule } = host.state();
  const name = rule.name || "this rule";
  if (!(await confirm("Delete the rule?", `"${name}" and all its versions are deleted permanently. This can't be undone.`, "Delete"))) return;
  try {
    await deleteRule(rule.id);
    toast(`"${name}" deleted.`);
    goBack();
  } catch (error) {
    const message = error instanceof BatchError ? `Could not delete the rule. Dataverse said: ${error.message}` : describeError("delete the rule", error);
    setStatus(statusLine(), message, "error");
  }
}

/** Starts a new, unsaved rule with the same settings and content as the version on screen. */
function copyRule(host: CommandHost): void {
  const source = host.state();
  host.setState({
    removed: noneRemoved(), replacesDraftId: null, ruleDirty: true, versionDirty: true,
    rule: {
      ...newRule(),
      name: `Copy of ${source.rule.name}`, ruleKey: `${source.rule.ruleKey}_COPY`,
      description: source.rule.description, enabled: source.rule.enabled,
    },
    version: { ...copyAsDraft(source.version, 1), derivedFromId: null },
  }, []);
  host.render();
  byId("tabBuilder").click();
  setStatus(statusLine(), "Copy ready. Check the name and rule key, then save to create the new rule.");
}

/** Shows the live version read-only, or goes back to the draft. */
async function toggleLive(host: CommandHost): Promise<void> {
  const state = host.state();
  if (viewing) {
    await host.load(state.rule.id);
    return;
  }
  const liveId = state.rule.publishedVersionId;
  if (!liveId) return;
  if (state.ruleDirty || state.versionDirty) {
    setStatus(statusLine(), "Save or discard your changes before viewing the live version.", "error");
    return;
  }
  try {
    host.setState({ ...state, version: await loadVersion(liveId) });
    viewing = true;
    host.render();
    byId("tabBuilder").click();
    document.querySelectorAll<HTMLInputElement>("#builder input, #builder select, #builder textarea, #builder button, #ruleName")
      .forEach((element) => { element.disabled = true; });
  } catch (error) {
    setStatus(statusLine(), describeError("load the live version", error), "error");
  }
}

export function bindDraftCommands(host: CommandHost): void {
  byId("cmdDiscard").addEventListener("click", () => void discard(host));
  byId("cmdDelete").addEventListener("click", () => void remove(host));
  byId("cmdCopy").addEventListener("click", () => copyRule(host));
  byId("cmdViewLive").addEventListener("click", () => void toggleLive(host));
}
