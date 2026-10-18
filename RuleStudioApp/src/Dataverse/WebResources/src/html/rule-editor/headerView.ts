// Page heading: the summary line under the rule name, the status pills and the editing notice.
import { countConditions, RuleEvent } from "./model";
import { byId, h, pill } from "../../shared/dom";
import { eventLabel, stageLabel } from "../../shared/labels";

import type { EditorState } from "./model";
import type { VersionListItem } from "./ruleData";

function versionNumber(versions: VersionListItem[], id: string | null): number | undefined {
  return versions.find((v) => v.id === id)?.versionNumber;
}

function statusPills(state: EditorState, versions: VersionListItem[]): HTMLElement[] {
  const { rule, version } = state;
  const pills: HTMLElement[] = [];
  const draft = versionNumber(versions, rule.draftVersionId);
  const live = versionNumber(versions, rule.publishedVersionId);
  if (version.isNew && state.versionDirty) pills.push(pill(`New draft v${version.versionNumber}`, "draft"));
  else if (draft !== undefined) pills.push(pill(`Draft v${draft}`, "draft"));
  pills.push(live === undefined ? pill("Not published", "warning") : pill(`Live v${live}`, "live"));
  if (!rule.enabled) pills.push(pill("Inactive", "warning"));
  if (state.ruleDirty || state.versionDirty) pills.push(pill("Unsaved changes", "info"));
  return pills;
}

export function renderHeader(state: EditorState, versions: VersionListItem[]): void {
  const { rule, version } = state;
  const trigger = version.targetTable
    ? `${version.targetTable} / ${eventLabel(version.event)} / ${stageLabel(version.stage)}`
    : "No trigger yet";
  byId("ruleSub").replaceChildren(h("span", { className: "mono" }, rule.ruleKey || "NO.KEY"), ` · ${trigger}`);

  byId("ruleStatus").replaceChildren(...statusPills(state, versions));

  byId("cmdActivate").textContent = rule.enabled ? "⏸ Deactivate" : "▶ Activate";
  const count = countConditions(version.root);
  byId("conditionCount").replaceChildren(pill(`${count} ${count === 1 ? "condition" : "conditions"}`, "info"));
}

function noticeText(state: EditorState, versions: VersionListItem[], liveNumber: number | undefined): string {
  const { rule, version } = state;
  if (rule.isNew) return "New rule: saving creates the rule and its first draft. Nothing runs until it is published.";
  if (version.isNew && state.replacesDraftId) {
    const old = versionNumber(versions, state.replacesDraftId);
    return `Unsaved draft v${version.versionNumber} copied from live v${liveNumber ?? "?"}. Saving replaces draft v${old ?? "?"}, which is kept in history as Discarded.`;
  }
  // Editing a copy of the live version needs no notice: the status pills already say Live vN.
  if (version.isNew && liveNumber !== undefined) return "";
  return version.isNew ? "This rule has no versions yet: saving creates draft v1." : "";
}

/** The notice above the builder that says which version the author is editing and what saving does. */
export function renderNotice(state: EditorState, versions: VersionListItem[], liveNumber: number | undefined, viewingLive = false): void {
  const notice = byId("editingNotice");
  const text = viewingLive
    ? `Viewing live v${liveNumber ?? "?"} (read-only). Click Back to draft to keep editing.`
    : noticeText(state, versions, liveNumber);
  notice.hidden = !text;
  notice.textContent = text;
  byId<HTMLInputElement>("ruleFiltered").placeholder = state.version.event === RuleEvent.Update ? "Any column" : "Update only";
}
