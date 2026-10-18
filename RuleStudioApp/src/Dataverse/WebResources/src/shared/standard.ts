// Page standard (docs/ui-standard.md): every page calls applyStandard() once at start.
// One-line descriptions (.rs-desc, .rs-intro) are cut with an ellipsis; this shows their full
// text as a tooltip, but only when the text is actually cut. Works for text set later too.
// It also starts the field help tooltips ([data-help], see tooltip.ts), page help (pageHelp.ts)
// and the back button (back.ts).
import { initBack } from "./back";
import { initPageHelp } from "./pageHelp";
import { initFieldHelp } from "./tooltip";

const ONE_LINE = ".rs-desc, .rs-intro";

function showFullText(event: Event): void {
  const element = (event.target as Element | null)?.closest<HTMLElement>(ONE_LINE);
  if (!element) return;
  const cut = element.scrollWidth > element.clientWidth;
  if (cut) element.title = element.textContent?.trim() ?? "";
  else element.removeAttribute("title");
}

export function applyStandard(): void {
  document.addEventListener("mouseover", showFullText);
  document.addEventListener("focusin", showFullText);
  initFieldHelp();
  initPageHelp();
  initBack();
}
