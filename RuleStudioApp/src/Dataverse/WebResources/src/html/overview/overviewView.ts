// Builds the overview cards from the loaded data. No queries here.
import { rs_release_rs_status as ReleaseStatus } from "../../../typings/XRM/OptionSets";
import { h, pill } from "../../shared/dom";
import { eventLabel, stageLabel } from "../../shared/labels";

import type { Failure, RecentDraft, ReleaseItem } from "./overviewData";
import type { RuleSummary } from "../../shared/data/rules";

const BAR_COLORS = ["var(--purple)", "var(--blue)", "var(--green)"];
const MAX_TRIGGERS = 6;

export function metric(icon: string, label: string, value: number, meta: string): HTMLDivElement {
  return h("div", { className: "metric" },
    h("span", { className: "ico", attrs: { "aria-hidden": "true" } }, icon),
    h("span", { className: "muted t12" }, label),
    h("div", { className: "num" }, String(value)),
    h("div", { className: "meta" }, meta));
}

export function emptyLine(text: string): HTMLParagraphElement {
  return h("p", { className: "empty overview-empty" }, text);
}

export function draftLine(draft: RecentDraft, onOpen: (ruleId: string) => void): HTMLDivElement {
  const open = h("button", { className: "btn mini", attrs: { type: "button" } }, "Continue");
  open.addEventListener("click", () => onOpen(draft.ruleId));
  return h("div", { className: "list-line" },
    h("div", { className: "grow" }, h("div", { className: "strong t12" }, draft.ruleName),
      h("div", { className: "muted t11 mono" }, draft.ruleKey), h("div", { className: "muted t11" }, `Modified ${draft.modified}`)),
    pill(`Draft v${draft.versionNumber ?? "?"}`, "draft"),
    open);
}

interface Trigger { label: string; count: number }

/** Rules per table · event · stage, using the live version when there is one, else the draft. */
export function triggers(rules: RuleSummary[], tableNames: Map<string, string>): Trigger[] {
  const counts = new Map<string, number>();
  for (const rule of rules) {
    const version = rule.live ?? rule.draft;
    if (!version?.targetTable) continue;
    const label = `${tableNames.get(version.targetTable) ?? version.targetTable} · ${eventLabel(version.event)} · ${stageLabel(version.stage)}`;
    counts.set(label, (counts.get(label) ?? 0) + 1);
  }
  return [...counts].map(([label, count]) => ({ label, count })).sort((a, b) => b.count - a.count).slice(0, MAX_TRIGGERS);
}

export function triggerBar(trigger: Trigger, max: number, index: number): HTMLDivElement {
  const fill = h("div", { className: "trigger-fill" });
  fill.style.width = `${Math.max(4, Math.round((trigger.count / max) * 100))}%`;
  fill.style.background = BAR_COLORS[index % BAR_COLORS.length];
  return h("div", { className: "trigger-row" },
    h("div", { className: "trigger-label" }, h("span", {}, trigger.label), h("b", {}, `${trigger.count} ${trigger.count === 1 ? "rule" : "rules"}`)),
    h("div", { className: "trigger-track" }, fill));
}

export function failureLine(failure: Failure): HTMLDivElement {
  return h("div", { className: "list-line" },
    pill("✕ Failed", "bad"),
    h("div", { className: "grow t12" }, h("b", {}, failure.ruleName), h("div", { className: "muted t11" }, failure.message)),
    h("span", { className: "muted t11" }, failure.when));
}

function releaseKind(status: number | null): "live" | "warning" | "bad" | "muted" {
  switch (status) {
    case ReleaseStatus.Live: return "live";
    case ReleaseStatus.Failed: return "bad";
    case ReleaseStatus.Rolledback: return "warning";
    default: return "muted";
  }
}

export function releaseLine(release: ReleaseItem): HTMLDivElement {
  return h("div", { className: "list-line" },
    h("div", { className: "grow t12" }, h("b", {}, release.name), h("div", { className: "muted t11" }, release.when)),
    pill(release.status || "—", releaseKind(release.statusValue)));
}
