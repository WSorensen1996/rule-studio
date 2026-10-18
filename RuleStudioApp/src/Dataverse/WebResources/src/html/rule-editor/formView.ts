// Sections 1 and 4: rule identity, trigger and runtime settings, bound to plain form fields.
// Name, key, description and enabled belong to the rule; everything else to the version.
import { FailurePolicy, LogLevel, RuleEvent, RunAs, Stage } from "./model";
import { byId, fillSelect } from "../../shared/dom";
import { eventLabel, stageLabel } from "../../shared/labels";

import type { EditorState, RuleModel, VersionModel } from "./model";

type RuleField = "name" | "ruleKey" | "description";
type VersionNumberField = "priority" | "retryCount" | "retryDelayMs";

const RULE_TEXT: [string, RuleField][] = [["ruleName", "name"], ["ruleKey", "ruleKey"], ["ruleDesc", "description"]];
const VERSION_NUMBERS: [string, VersionNumberField][] = [["vPriority", "priority"], ["vRetries", "retryCount"], ["vDelay", "retryDelayMs"]];
const VERSION_CHOICES: [string, "event" | "stage" | "failurePolicy" | "runAs"][] = [
  ["ruleEvent", "event"], ["ruleStage", "stage"], ["vFailure", "failurePolicy"], ["vRunAs", "runAs"],
];

export interface FormHandlers {
  state: () => EditorState;
  ruleChanged: () => void;
  /** structural = conditions and actions must be drawn again (table, event or stage changed). */
  versionChanged: (structural: boolean) => void;
}

function input(id: string): HTMLInputElement {
  return byId<HTMLInputElement>(id);
}

/** A text input or a select: both have value and disabled. */
function control(id: string): HTMLInputElement | HTMLSelectElement {
  return byId<HTMLInputElement | HTMLSelectElement>(id);
}

function keyPart(text: string): string {
  return text.normalize("NFKD").replace(/[^\w\s-]/g, "").trim().replace(/[\s-]+/g, "_").toUpperCase();
}

/** TABLE.RULE_NAME, e.g. QUOTE.ANALYTICS_REQUIRES_CORE. */
export function suggestKey(rule: RuleModel, table: string): string {
  return `${keyPart(table) || "GENERAL"}.${keyPart(rule.name) || "NEW_RULE"}`;
}

function fillChoices(): void {
  const options = (values: Record<string, number>, label: (v: number) => string): { value: string; label: string }[] =>
    Object.values(values).map((v) => ({ value: String(v), label: label(v) }));
  fillSelect(byId("ruleEvent"), options(RuleEvent, eventLabel));
  fillSelect(byId("ruleStage"), options(Stage, stageLabel));
  fillSelect(byId("vFailure"), [{ value: String(FailurePolicy.Block), label: "Block" }, { value: String(FailurePolicy.Logandcontinue), label: "Log and continue" }]);
  fillSelect(byId("vRunAs"), [{ value: String(RunAs.Callinguser), label: "Calling user" }, { value: String(RunAs.System), label: "System" }]);
  fillSelect(byId("vLog"), [
    { value: "", label: "Default (errors only)" },
    { value: String(LogLevel.Off), label: "Off" }, { value: String(LogLevel.Errorsonly), label: "Errors only" },
    { value: String(LogLevel.Summary), label: "Summary" }, { value: String(LogLevel.Fulltrace), label: "Full trace" },
  ]);
}

/** Writes the model into the form. */
export function fillForm(state: EditorState): void {
  const { rule, version } = state;
  for (const [id, key] of RULE_TEXT) control(id).value = rule[key];
  input("ruleTable").value = version.targetTable;
  input("ruleFiltered").value = version.filteredColumns;
  input("vStop").checked = version.stopProcessing;
  input("vNote").value = version.changeNote;
  for (const [id, key] of VERSION_NUMBERS) input(id).value = String(version[key]);
  for (const [id, key] of VERSION_CHOICES) control(id).value = String(version[key]);
  control("vLog").value = version.logLevel === null ? "" : String(version.logLevel);
  input("ruleFiltered").disabled = input("ruleFiltered").disabled || version.event !== RuleEvent.Update;
}

function bindRule(handlers: FormHandlers): void {
  let keyTouched = false;
  for (const [id, key] of RULE_TEXT) {
    control(id).addEventListener("input", () => {
      const { rule } = handlers.state();
      rule[key] = control(id).value;
      if (id === "ruleKey") keyTouched = true;
      if (rule.isNew && !keyTouched && id !== "ruleKey" && id !== "ruleDesc") {
        rule.ruleKey = suggestKey(rule, handlers.state().version.targetTable);
        input("ruleKey").value = rule.ruleKey;
      }
      handlers.ruleChanged();
    });
  }
  byId("cmdActivate").addEventListener("click", () => {
    const rule = handlers.state().rule;
    rule.enabled = !rule.enabled;
    handlers.ruleChanged();
  });
}

function bindVersion(handlers: FormHandlers): void {
  const version = (): VersionModel => handlers.state().version;
  input("ruleTable").addEventListener("change", () => {
    version().targetTable = input("ruleTable").value.trim().toLowerCase();
    input("ruleTable").value = version().targetTable;
    handlers.versionChanged(true);
  });
  for (const [id, key] of VERSION_CHOICES) {
    control(id).addEventListener("change", () => {
      version()[key] = Number(control(id).value);
      if (key === "event") input("ruleFiltered").disabled = version().event !== RuleEvent.Update;
      handlers.versionChanged(key === "event" || key === "stage");
    });
  }
  control("vLog").addEventListener("change", () => {
    const value = control("vLog").value;
    version().logLevel = value === "" ? null : Number(value);
    handlers.versionChanged(false);
  });
  for (const [id, key] of VERSION_NUMBERS) {
    input(id).addEventListener("change", () => {
      const value = Math.max(0, Math.round(Number(input(id).value) || 0));
      version()[key] = value;
      input(id).value = String(value);
      handlers.versionChanged(false);
    });
  }
  input("ruleFiltered").addEventListener("change", () => {
    version().filteredColumns = input("ruleFiltered").value.split(",").map((c) => c.trim().toLowerCase()).filter(Boolean).join(",");
    input("ruleFiltered").value = version().filteredColumns;
    handlers.versionChanged(false);
  });
  input("vStop").addEventListener("change", () => {
    version().stopProcessing = input("vStop").checked;
    handlers.versionChanged(false);
  });
  input("vNote").addEventListener("input", () => {
    version().changeNote = input("vNote").value;
    handlers.versionChanged(false);
  });
}

/** Attaches the form listeners once. */
export function bindForm(handlers: FormHandlers): void {
  fillChoices();
  bindRule(handlers);
  bindVersion(handlers);
}
