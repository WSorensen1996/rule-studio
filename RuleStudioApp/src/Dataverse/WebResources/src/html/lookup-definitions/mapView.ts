// The "how the fields connect" picture: record fields on the left, lookup table columns on the
// right, a line per match criterion and an arrow for the column that comes back.
import { valueText } from "./criteriaText";
import { h } from "../../shared/dom";
import { findColumn, tableLabel } from "../../shared/tableMetadata";

import type { CriterionValueSource, MatchCriterion } from "../../shared/formats";

export interface MapCriterion {
  operator: MatchCriterion["operator"];
  /** Lookup table columns (min and max for a range); empty strings while not chosen. */
  columns: string[];
  /** Record-side values; incomplete ones are left out. */
  values: CriterionValueSource[];
  wildcard: boolean;
}

export interface MapSpec {
  recordTable: string;
  lookupTable: string;
  returnColumn: string;
  criteria: MapCriterion[];
}

const SVG = "http://www.w3.org/2000/svg";

/** "record → Quote → Country" becomes "Country" with the note "via Quote". */
async function recordText(source: CriterionValueSource): Promise<[string, string]> {
  const text = await valueText(source);
  if (source.type !== "fieldPath") return [text, source.type === "fixed" ? "fixed value" : "expression"];
  const parts = text.split(" → ").slice(1);
  const last = parts.pop() ?? text;
  return [last, parts.length ? `via ${parts.join(" → ")}` : ""];
}

function field(className: string, text: string, note: string): HTMLElement {
  return h("div", { className: `map-field ${className}` }, h("span", {}, text), note ? h("span", { className: "map-note" }, note) : null);
}

function header(title: string, hint: string): HTMLElement {
  return h("div", { className: "map-head" }, h("span", {}, title), h("small", {}, hint));
}

function columnNote(criterion: MapCriterion, index: number): string {
  if (criterion.operator === "range") return index === 0 ? "min" : "max";
  if (criterion.operator === "isEmpty") return "must be empty";
  return criterion.wildcard ? "empty = any" : "";
}

type Link = [from: Element, to: Element, className: string];

function curve(svg: SVGSVGElement, box: DOMRect, [from, to, className]: Link): void {
  const a = from.getBoundingClientRect();
  const b = to.getBoundingClientRect();
  const forward = a.left < b.left;
  const x1 = (forward ? a.right : a.left) - box.left;
  const x2 = (forward ? b.left : b.right) - box.left;
  const y1 = a.top + a.height / 2 - box.top;
  const y2 = b.top + b.height / 2 - box.top;
  const mid = (x1 + x2) / 2;
  const path = document.createElementNS(SVG, "path");
  path.setAttribute("d", `M${x1},${y1} C${mid},${y1} ${mid},${y2} ${x2},${y2}`);
  path.setAttribute("class", className);
  svg.append(path);
}

export function renderMap(spec: MapSpec): HTMLElement {
  if (!spec.recordTable || !spec.lookupTable) {
    return h("p", { className: "muted t11" }, "Choose both tables to see how the fields connect.");
  }
  const labels: Promise<unknown>[] = [];
  const relabel = (element: HTMLElement, text: Promise<string>): void => {
    labels.push((async () => {
      element.firstElementChild!.textContent = await text;
    })().catch(() => null));
  };
  const leftHead = header(spec.recordTable, "the record being saved");
  const rightHead = header(spec.lookupTable, "the table with the answers");
  relabel(leftHead, tableLabel(spec.recordTable));
  relabel(rightHead, tableLabel(spec.lookupTable));
  const left = h("div", { className: "map-table" }, leftHead);
  const right = h("div", { className: "map-table" }, rightHead);

  // Each lookup column appears once on the right, however many criteria use it.
  const columnFields = new Map<string, HTMLElement>();
  const columnField = (column: string, className: string, note: string): HTMLElement => {
    let element = columnFields.get(column);
    if (!element) {
      element = field(className, column, note);
      relabel(element, findColumn(spec.lookupTable, column).then((c) => c?.label ?? column));
      columnFields.set(column, element);
      right.append(element);
    }
    return element;
  };

  const links: Link[] = [];
  for (const criterion of spec.criteria) {
    const targets = criterion.columns.flatMap((column, i) => column ? [columnField(column, "is-match", columnNote(criterion, i))] : []);
    for (const value of criterion.values) {
      const source = field("is-match", "…", "");
      labels.push((async () => {
        const [text, note] = await recordText(value);
        source.replaceChildren(...Array.from(field("is-match", text, note).childNodes));
      })().catch(() => null));
      left.append(source);
      for (const target of targets) links.push([source, target, "map-link"]);
    }
  }
  if (spec.criteria.length === 0) left.append(field("", "No criteria", "every row matches"));
  if (spec.returnColumn) {
    const answer = columnField(spec.returnColumn, "is-answer", "⇒ answer");
    answer.className = "map-field is-answer";
    const result = field("is-answer", "Lookup result", "used by the rule");
    left.append(result);
    links.push([answer, result, "map-link answer"]);
  }

  const svg = document.createElementNS(SVG, "svg");
  svg.setAttribute("aria-hidden", "true");
  const map = h("div", { className: "lookup-map", attrs: { role: "img", "aria-label": "How the record fields connect to the lookup table" } }, left, h("div"), right);
  map.append(svg);
  const draw = (): void => {
    const box = map.getBoundingClientRect();
    svg.replaceChildren();
    if (box.width === 0) return;
    for (const link of links) curve(svg, box, link);
  };
  new ResizeObserver(draw).observe(map);
  void Promise.all(labels).then(draw);

  const legend = h("div", { className: "map-legend t11" },
    h("span", {}, h("i", { className: "map-swatch" }), "must match"),
    h("span", {}, h("i", { className: "map-swatch answer" }), "answer comes back"));
  return h("div", { className: "map-wrap" }, map, legend);
}
