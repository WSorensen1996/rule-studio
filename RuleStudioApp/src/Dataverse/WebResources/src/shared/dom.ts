// Small DOM helpers. Pages build elements with these instead of innerHTML, so record data
// can never be interpreted as markup.

type Child = Node | string | null | undefined | false;

/** Creates an element with optional class, attributes and children. */
export function h<K extends keyof HTMLElementTagNameMap>(
  tag: K,
  props: { className?: string; attrs?: Record<string, string> } = {},
  ...children: Child[]
): HTMLElementTagNameMap[K] {
  const element = document.createElement(tag);
  if (props.className) element.className = props.className;
  for (const [name, value] of Object.entries(props.attrs ?? {})) element.setAttribute(name, value);
  for (const child of children) if (child) element.append(child);
  return element;
}

/** Returns the element with this id, typed; throws if the page markup is missing it. */
export function byId<T extends HTMLElement = HTMLElement>(id: string): T {
  const element = document.querySelector<T>(`#${id}`);
  if (!element) throw new Error(`Element #${id} is missing from the page.`);
  return element;
}

export function pill(text: string, kind: "live" | "draft" | "warning" | "bad" | "info" | "muted"): HTMLSpanElement {
  return h("span", { className: `pill ${kind}` }, text);
}

/** Fills a <select> with options, keeping the current choice when it still exists. */
export function fillSelect(select: HTMLSelectElement, options: { value: string; label: string }[]): void {
  const current = select.value;
  select.replaceChildren(...options.map(({ value, label }) => h("option", { attrs: { value } }, label)));
  if (options.some((option) => option.value === current)) select.value = current;
}
