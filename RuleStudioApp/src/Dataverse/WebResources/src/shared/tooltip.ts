// Field help tooltips (docs/ui-standard.md, "Field help"). One bubble per page, shown after
// hovering or focusing a [data-help] control for 2 seconds. Started by applyStandard().
import { FIELD_HELP } from "./fieldHelp";

import type { HelpKey } from "./fieldHelp";

const DELAY_MS = 2000;
let bubble: HTMLDivElement | null = null;
let timer = 0;
let current: HTMLElement | null = null;

/** Tags a control built in code with a help key; returns the same element. */
export function withHelp<T extends HTMLElement>(element: T, key: HelpKey): T {
  element.dataset.help = key;
  return element;
}

function ensureBubble(): HTMLDivElement {
  if (bubble) return bubble;
  bubble = document.createElement("div");
  bubble.className = "rs-tip";
  bubble.id = "rs-tip";
  bubble.setAttribute("role", "tooltip");
  bubble.hidden = true;
  document.body.append(bubble);
  return bubble;
}

function place(tip: HTMLDivElement, target: HTMLElement): void {
  const box = target.getBoundingClientRect();
  const width = tip.offsetWidth;
  const left = Math.max(8, Math.min(box.left, window.innerWidth - width - 8));
  const below = box.bottom + 6;
  const top = below + tip.offsetHeight > window.innerHeight ? box.top - tip.offsetHeight - 6 : below;
  tip.style.left = `${left}px`;
  tip.style.top = `${Math.max(8, top)}px`;
}

function show(target: HTMLElement): void {
  const text = FIELD_HELP[target.dataset.help as HelpKey];
  if (!text || !target.isConnected) return;
  const tip = ensureBubble();
  tip.textContent = text;
  tip.hidden = false;
  place(tip, target);
  target.setAttribute("aria-describedby", tip.id);
}

function hide(): void {
  window.clearTimeout(timer);
  current?.removeAttribute("aria-describedby");
  current = null;
  if (bubble) bubble.hidden = true;
}

function start(event: Event): void {
  const target = (event.target as Element | null)?.closest<HTMLElement>("[data-help]") ?? null;
  if (target === current) return;
  hide();
  if (!target) return;
  current = target;
  timer = window.setTimeout(() => show(target), DELAY_MS);
}

export function initFieldHelp(): void {
  document.addEventListener("mouseover", start);
  document.addEventListener("focusin", start);
  document.addEventListener("focusout", hide);
  document.addEventListener("mousedown", hide);
  document.addEventListener("keydown", (event) => { if (event.key === "Escape") hide(); });
  window.addEventListener("scroll", hide, true);
}
