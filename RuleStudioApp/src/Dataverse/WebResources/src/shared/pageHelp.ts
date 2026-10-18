// Page help (docs/ui-standard.md, "Page help"). Background information lives behind a "?" icon
// at the far right of the command bar (page level), a section header or a dialog intro, never in an always-visible box:
//   <details class="rs-help"><summary aria-label="About …">?</summary>
//     <div class="rs-help-pop"><strong>Title</strong>Text</div></details>
// The markup works without script; this only keeps one open and closes it on Esc or an outside click.
const OPEN = "details.rs-help[open]";

function closeAll(except?: Element | null): void {
  document.querySelectorAll<HTMLDetailsElement>(OPEN).forEach((d) => { if (d !== except) d.open = false; });
}

// The command bar scrolls sideways, which would clip the popover; there it is fixed to the icon.
function placeFixed(details: HTMLDetailsElement): void {
  const pop = details.querySelector<HTMLElement>(".rs-help-pop");
  if (!pop || !details.open || !details.closest(".rs-commandbar")) return;
  const box = details.getBoundingClientRect();
  pop.style.top = `${box.bottom + 6}px`;
  pop.style.right = `${Math.max(8, window.innerWidth - box.right)}px`;
}

export function initPageHelp(): void {
  document.querySelectorAll<HTMLDetailsElement>("details.rs-help").forEach((d) => d.addEventListener("toggle", () => placeFixed(d)));
  document.addEventListener("click", (event) => closeAll((event.target as Element | null)?.closest("details.rs-help")));
  document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") return;
    const open = document.querySelector<HTMLDetailsElement>(OPEN);
    if (!open) return;
    open.open = false;
    open.querySelector("summary")?.focus();
  });
}
