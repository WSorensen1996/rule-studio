// The back button (docs/ui-standard.md, "Records open full-page"): the first command in every
// page's command bar. Started by applyStandard(). A record page with unsaved changes registers
// a leave guard, and Back then asks first: save and leave, leave without saving, or stay.
import { h } from "./dom";
import { goBack, hasBack } from "./nav";

export interface LeaveGuard {
  /** True when the page holds changes that are not saved. */
  dirty: () => boolean;
  /** Saves; resolves true when the save worked and the page may be left. */
  save: () => Promise<boolean>;
}

let guard: LeaveGuard | undefined;

/** Called once by a record page that can hold unsaved changes. */
export function setLeaveGuard(next: LeaveGuard): void {
  guard = next;
}

type Choice = "save" | "discard" | "stay";

function askToLeave(): Promise<Choice> {
  return new Promise((resolve) => {
    const button = (label: string, choice: Choice, primary = false): HTMLButtonElement => {
      const element = h("button", { className: primary ? "btn primary" : "btn", attrs: { type: "button" } }, label);
      element.addEventListener("click", () => dialog.close(choice));
      return element;
    };
    const dialog = h("dialog", { className: "rs-leave", attrs: { "aria-labelledby": "rsLeaveTitle" } },
      h("h2", { className: "h3", attrs: { id: "rsLeaveTitle" } }, "Unsaved changes"),
      h("p", {}, "You have changes on this page that are not saved."),
      h("div", { className: "rs-leave-foot" },
        button("Save and leave", "save", true), button("Leave without saving", "discard"), button("Stay", "stay")));
    dialog.addEventListener("close", () => {
      dialog.remove();
      resolve((dialog.returnValue || "stay") as Choice);
    });
    document.body.append(dialog);
    dialog.showModal();
  });
}

/** Back, Save & close and Delete leave the page through here, so the unsaved-changes check runs once. */
export async function leavePage(): Promise<void> {
  if (guard?.dirty()) {
    const choice = await askToLeave();
    if (choice === "stay") return;
    if (choice === "save" && !(await guard.save())) return;
  }
  goBack();
}

/** Save & close: saves, and goes back only when the save worked. */
export async function goBackAfterSave(save: () => Promise<boolean>): Promise<void> {
  if (await save()) goBack();
}

// The model-driven app's back arrow (Fluent "Arrow left"), drawn at the same size and weight.
const ARROW = "M9.16 16.87a.5.5 0 1 0 .67-.74L3.67 10.5H17.5a.5.5 0 0 0 0-1H3.67l6.16-5.63a.5.5 0 0 0-.67-.74L2.24 9.44a.75.75 0 0 0 0 1.12l6.92 6.31Z";

function arrow(): SVGSVGElement {
  const ns = "http://www.w3.org/2000/svg";
  const svg = document.createElementNS(ns, "svg");
  svg.setAttribute("viewBox", "0 0 20 20");
  svg.setAttribute("aria-hidden", "true");
  const path = document.createElementNS(ns, "path");
  path.setAttribute("d", ARROW);
  path.setAttribute("fill", "currentColor");
  svg.append(path);
  return svg;
}

/**
 * Binds #cmdBack, if the page has one. With nowhere to go back to (a list page opened from the
 * sitemap) the button is removed rather than greyed out; otherwise it gets a divider after it.
 */
export function initBack(): void {
  const back = document.getElementById("cmdBack") as HTMLButtonElement | null;
  if (!back) return;
  if (!hasBack()) {
    const bar = back.parentElement;
    back.remove();
    // A bar that only held Back (Overview, Documentation) goes too.
    if (bar?.classList.contains("rs-commandbar") && !bar.querySelector("button, details")) bar.remove();
    return;
  }
  back.replaceChildren(arrow());
  back.after(h("span", { className: "command-divider", attrs: { "aria-hidden": "true" } }));
  back.addEventListener("click", () => void leavePage());
}
