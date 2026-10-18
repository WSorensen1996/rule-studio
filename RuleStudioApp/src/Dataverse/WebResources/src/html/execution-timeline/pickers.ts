// Table → event → stage pickers, each narrowed to the triggers that exist on rule versions.
import { fillSelect } from "../../shared/dom";
import { eventLabel, stageLabel } from "../../shared/labels";

import type { Trigger } from "./chainModel";
import type { Params } from "../../shared/nav";

export interface Pickers {
  table: HTMLSelectElement;
  event: HTMLSelectElement;
  stage: HTMLSelectElement;
}

type Option = { value: string; label: string };

function distinct(values: number[], label: (value: number) => string): Option[] {
  return [...new Set(values)].sort((a, b) => a - b).map((value) => ({ value: String(value), label: label(value) }));
}

/** Refills the pickers from the triggers, keeping each choice while it is still available. */
export function fillPickers(pickers: Pickers, triggers: Trigger[], tableNames: Map<string, string>): void {
  const name = (table: string): string => tableNames.get(table) ?? table;
  const tables = [...new Set(triggers.map((t) => t.table))].sort((a, b) => name(a).localeCompare(name(b)));
  fillSelect(pickers.table, tables.map((table) => ({ value: table, label: name(table) })));
  const forTable = triggers.filter((t) => t.table === pickers.table.value);
  fillSelect(pickers.event, distinct(forTable.map((t) => t.event), eventLabel));
  const forEvent = forTable.filter((t) => String(t.event) === pickers.event.value);
  fillSelect(pickers.stage, distinct(forEvent.map((t) => t.stage), stageLabel));
}

/** The trigger the pickers show, or null when nothing can be picked. */
export function readTrigger(pickers: Pickers): Trigger | null {
  const event = Number(pickers.event.value);
  const stage = Number(pickers.stage.value);
  if (!pickers.table.value || !pickers.event.value || !pickers.stage.value) return null;
  return { table: pickers.table.value, event, stage };
}

/**
 * Pre-selects a trigger passed as page parameters (table, event and stage as option values).
 * Called after the first fill, then the pickers are filled again to narrow the lists.
 */
export function applyTriggerParams(pickers: Pickers, params: Params, refill: () => void): void {
  if (params.table) pickers.table.value = params.table;
  refill();
  if (params.event) pickers.event.value = params.event;
  refill();
  if (params.stage) pickers.stage.value = params.stage;
}
