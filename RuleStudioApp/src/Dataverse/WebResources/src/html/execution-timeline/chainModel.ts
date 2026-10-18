// Builds an execution chain from authored data: the rules whose chosen version matches a
// trigger, by ascending priority (ties by rule key), with live/draft differences and the
// stop-processing cut-off. No engine is involved; this mirrors what the engine is meant to do.
import type { ChainRule, ChainVersion } from "./chainData";

export type ViewMode = "live" | "draft";

export interface Trigger {
  table: string;
  event: number;
  stage: number;
}

/** How a step differs from the other view: added, removed, changed (priority/stop/columns) or not at all. */
export type StepDiff = "none" | "added" | "removed" | "changed";

export interface ChainStep {
  rule: ChainRule;
  version: ChainVersion;
  diff: StepDiff;
  active: boolean;
  cutOff: boolean;
}

export function triggerKey(trigger: Trigger): string {
  return `${trigger.table}|${trigger.event}|${trigger.stage}`;
}

function versionTrigger(version: ChainVersion | null): Trigger | null {
  if (!version?.targetTable || version.event === null || version.stage === null) return null;
  return { table: version.targetTable, event: version.event, stage: version.stage };
}

/** Distinct triggers found on any live or draft version. */
export function distinctTriggers(rules: ChainRule[]): Trigger[] {
  const found = new Map<string, Trigger>();
  for (const rule of rules) {
    for (const trigger of [versionTrigger(rule.live), versionTrigger(rule.draft)]) {
      if (trigger) found.set(triggerKey(trigger), trigger);
    }
  }
  return [...found.values()];
}

/** The version a view uses: live, or in draft view the draft where one exists. */
export function versionFor(rule: ChainRule, mode: ViewMode): ChainVersion | null {
  return mode === "draft" ? (rule.draft ?? rule.live) : rule.live;
}

function matchesTrigger(version: ChainVersion | null, trigger: Trigger): version is ChainVersion {
  const own = versionTrigger(version);
  return own !== null && triggerKey(own) === triggerKey(trigger);
}

function sameBehaviour(a: ChainVersion, b: ChainVersion): boolean {
  return a.priority === b.priority && a.stopProcessing === b.stopProcessing && a.filteredColumns === b.filteredColumns;
}

function diffFor(own: ChainVersion, other: ChainVersion | null, trigger: Trigger): StepDiff {
  if (!matchesTrigger(other, trigger)) return "added";
  return sameBehaviour(own, other) ? "none" : "changed";
}

// Rule keys compare as the engine compares them: ordinal, ignoring case (not locale order).
function byKey(a: string, b: string): number {
  const x = a.toUpperCase();
  const y = b.toUpperCase();
  if (x === y) return 0;
  return x < y ? -1 : 1;
}

function byPriority(a: ChainStep, b: ChainStep): number {
  const pa = a.version.priority ?? Number.MAX_SAFE_INTEGER;
  const pb = b.version.priority ?? Number.MAX_SAFE_INTEGER;
  return pa === pb ? byKey(a.rule.ruleKey, b.rule.ruleKey) : pa - pb;
}

function markCutOff(steps: ChainStep[]): void {
  let stopped = false;
  for (const step of steps) {
    step.cutOff = stopped;
    if (step.active && step.version.stopProcessing) stopped = true;
  }
}

/** The chain for a trigger in one view, sorted, plus the rules only the other view runs here. */
export function buildChain(rules: ChainRule[], trigger: Trigger, mode: ViewMode): { steps: ChainStep[]; others: ChainStep[] } {
  const steps: ChainStep[] = [];
  const others: ChainStep[] = [];
  const otherMode: ViewMode = mode === "live" ? "draft" : "live";
  for (const rule of rules) {
    const own = versionFor(rule, mode);
    const other = versionFor(rule, otherMode);
    const active = rule.enabled;
    if (matchesTrigger(own, trigger)) {
      steps.push({ rule, version: own, diff: diffFor(own, other, trigger), active, cutOff: false });
    } else if (matchesTrigger(other, trigger)) {
      others.push({ rule, version: other, diff: "removed", active, cutOff: false });
    }
  }
  steps.sort(byPriority);
  others.sort(byPriority);
  markCutOff(steps);
  return { steps, others };
}
