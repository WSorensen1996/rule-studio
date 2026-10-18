// Display labels for the Rule Studio choices, keyed by the generated option values so a
// changed value in Dataverse breaks the build instead of showing a wrong label.
import { rs_event, rs_stage } from "../../typings/XRM/OptionSets";

const EVENT_LABELS: Record<number, string> = {
  [rs_event.Create]: "Create",
  [rs_event.Update]: "Update",
  [rs_event.Delete]: "Delete",
};

const STAGE_LABELS: Record<number, string> = {
  [rs_stage.Beforesave]: "Before save",
  [rs_stage.Aftersave]: "After save",
};

export function eventLabel(value: number | null | undefined): string {
  return value == null ? "—" : (EVENT_LABELS[value] ?? String(value));
}

export function stageLabel(value: number | null | undefined): string {
  return value == null ? "—" : (STAGE_LABELS[value] ?? String(value));
}
