// Version history tab and section 5 (observed activity from the execution logs).
import { Filter, XrmQuery } from "@delegateas/xrmquery";

import { VersionStatus } from "./model";
import { h, pill } from "../../shared/dom";

import type { EditorState } from "./model";
import type { VersionListItem } from "./ruleData";

const RECENT_LOGS = 20;

function statusPill(item: VersionListItem, state: EditorState): HTMLSpanElement {
  if (item.id === state.rule.draftVersionId) return pill("Draft", "draft");
  if (item.id === state.rule.publishedVersionId) return pill("Live", "live");
  if (item.status === VersionStatus.Discarded) return pill("Discarded", "muted");
  return pill(item.statusLabel || "—", item.status === VersionStatus.Draft ? "draft" : "muted");
}

function title(item: VersionListItem, state: EditorState): string {
  const parts = [`Version ${item.versionNumber}`];
  if (item.id === state.rule.draftVersionId) parts.push("Current working draft");
  if (item.id === state.rule.publishedVersionId) parts.push("Currently live in this environment");
  if (item.id === state.version.id && !state.version.isNew) parts.push("Open in the editor");
  return parts.join(" · ");
}

export function renderHistory(
  container: HTMLElement,
  versions: VersionListItem[],
  state: EditorState,
  options: { onRestore: (item: VersionListItem) => void; author: boolean },
): void {
  if (versions.length === 0) {
    container.replaceChildren(h("p", { className: "muted t12" }, "No saved versions yet. Save the draft to create version 1."));
    return;
  }
  container.replaceChildren(...versions.map((item) => {
    const meta = [`Priority ${item.priority ?? "—"}`];
    if (item.publishedOn) meta.push(`Published ${item.publishedOn}${item.publishedBy ? ` by ${item.publishedBy}` : ""}`);
    const line = h("div", { className: "list-line" },
      h("div", { className: "grow" }, h("b", {}, title(item, state)), h("div", { className: "muted t11" }, meta.join(" · ")),
        h("div", { className: "muted t11" }, item.changeNote || "No change note")),
      statusPill(item, state));
    if (item.status === VersionStatus.Published || item.status === VersionStatus.Superseded) {
      const restore = h("button", { className: "btn mini", attrs: { type: "button" } }, "Restore as draft");
      restore.disabled = !options.author;
      if (!options.author) restore.title = "Requires the Rule Studio Author role.";
      restore.addEventListener("click", () => options.onRestore(item));
      line.append(restore);
    }
    return line;
  }));
}

export async function renderActivity(container: HTMLElement, ruleId: string | null): Promise<void> {
  if (!ruleId) {
    container.replaceChildren(h("p", { className: "muted t12" }, "Activity appears here once the rule is published and runs."));
    return;
  }
  const logs = await XrmQuery.retrieveMultiple((x) => x.rs_executionlogs)
    .select((x) => [x.rs_durationms, x.rs_outcome, x.createdon])
    .filter((x) => Filter.and(Filter.equals(x.rs_ruleid_guid, Filter.makeGuid(ruleId)), Filter.notEquals(x.rs_istest, true)))
    .orderDesc((x) => x.createdon)
    .top(RECENT_LOGS)
    .includeFormattedValues()
    .execute();
  if (logs.length === 0) {
    container.replaceChildren(h("p", { className: "muted t12" }, "No production execution logs for this rule yet. With log level Errors only, successful runs are not logged."));
    return;
  }
  const durations = logs.map((l) => l.rs_durationms ?? 0);
  const average = Math.round(durations.reduce((a, b) => a + b, 0) / durations.length);
  const longest = Math.max(...durations, 1);
  const bars = [...logs].reverse().map((l) => {
    const bar = h("div", { className: "activity-bar", attrs: { title: `${l.rs_outcome_formatted ?? ""} · ${l.rs_durationms ?? 0} ms · ${l.createdon_formatted ?? ""}` } });
    bar.style.height = `${Math.max(8, Math.round(((l.rs_durationms ?? 0) / longest) * 100))}%`;
    return bar;
  });
  const metric = (heading: string, value: string, unit: string, note: string): HTMLElement =>
    h("div", { className: "activity-metric" }, h("span", { className: "activity-heading" }, heading),
      h("strong", { className: "activity-number" }, `${value} `, h("small", {}, unit)), h("span", { className: "muted t11" }, note));
  container.replaceChildren(
    h("div", { className: "activity-grid" },
      metric("AVERAGE LOGGED LATENCY", String(average), "ms", `Average across the last ${logs.length} logged runs`),
      metric("OBSERVED EXECUTIONS", String(logs.length), logs.length === 1 ? "log" : "logs", `Most recent ${RECENT_LOGS} retained production logs · not total triggers`),
      h("div", { className: "activity-trend" },
        h("div", { className: "trend-head" }, h("b", { className: "t12" }, "Recent durations"), h("span", { className: "muted t11" }, "ms per logged run")),
        h("div", { className: "activity-bars" }, ...bars))),
    h("p", { className: "activity-disclaimer" }, "Frequency is a log count, not a trigger rate: the logging level may omit successful runs, and retention removes older rows."));
}
