// Opening Rule Studio pages and dialogs with Xrm.Navigation.navigateTo (docs/ui-standard.md).
// Records (a rule, a function, a release …) open full-page in the main area, like a
// model-driven form, and have a back button. Dialogs are only for confirm-and-act steps;
// their sizes live here, in one place, so every opener shows a dialog the same way.
import { hostXrm } from "./xrm";

const PREFIX = "rs_RuleStudio/html/";

interface DialogSize {
  title: string;
  /** 1 = centered dialog, 2 = side pane. */
  position: 1 | 2;
  /** Percent when ≤ 100, otherwise pixels. */
  width: number;
  /** Percent. Side panes take the full height. */
  height?: number;
}

export const DIALOGS = {
  "publish-release": { title: "Validate & publish release", position: 1, width: 760, height: 80 },
  "rollback-release": { title: "Rollback release", position: 1, width: 600, height: 60 },
  "restore-version": { title: "Restore a previous version", position: 1, width: 560, height: 45 },
} as const satisfies Record<string, DialogSize>;

export type DialogName = keyof typeof DIALOGS;
export type PageName =
  | "overview" | "rule-library" | "execution-timeline" | "test-trace"
  | "custom-functions" | "lookup-definitions" | "releases" | "execution-logs" | "environments" | "guide";

/** Record pages and the list each one belongs to: Back goes there when there is no page to return to. */
export const RECORD_PAGES = {
  "rule-editor": "rule-library",
  "register-function": "custom-functions",
  "new-lookup-definition": "lookup-definitions",
  "release-detail": "releases",
  "log-detail": "execution-logs",
} as const satisfies Record<string, PageName>;

export type RecordPageName = keyof typeof RECORD_PAGES;

export type Params = Record<string, string>;

/** Set on every page opened from another Rule Studio page, so Back knows there is one to return to. */
const FROM_APP = "back";

function encode(params: Params | undefined): string {
  return params ? new URLSearchParams(params).toString() : "";
}

function requireXrm(): Xrm.XrmStatic {
  const xrm = hostXrm();
  if (!xrm) throw new Error("Rule Studio pages must be opened inside the Rule Studio app.");
  return xrm;
}

/** Parameters this page was opened with (the web resource `data` query parameter). */
export function pageParams(): Params {
  const raw = new URLSearchParams(window.location.search).get("data") ?? "";
  const params: Params = {};
  new URLSearchParams(raw).forEach((value, key) => {
    params[key] = value;
  });
  return params;
}

/** Opens a page in the main area, replacing the current one (a new entry in the browser history). */
export async function navigateToPage(page: PageName | RecordPageName, params?: Params): Promise<void> {
  await requireXrm().Navigation.navigateTo(
    { pageType: "webresource", webresourceName: `${PREFIX}${page}.html`, data: encode({ ...params, [FROM_APP]: "1" }) },
    { target: 1 },
  );
}

/** Opens a record full-page in the main area, the way a model-driven app opens a form. */
export async function openRecord(page: RecordPageName, params?: Params): Promise<void> {
  await navigateToPage(page, params);
}

/** The record page this page is, or undefined for a list page. */
function recordPage(): RecordPageName | undefined {
  const file = /([\w-]+)\.html/.exec(window.location.pathname)?.[1];
  return file && file in RECORD_PAGES ? (file as RecordPageName) : undefined;
}

/** True when this page was opened from another page and the browser can go back to it. */
export function canGoBack(): boolean {
  return pageParams()[FROM_APP] === "1" && window.history.length > 1;
}

/** True when Back does something: there is a page to return to, or this is a record page. */
export function hasBack(): boolean {
  return canGoBack() || recordPage() !== undefined;
}

/**
 * Goes back to the page this one was opened from. A record opened from a link or a bookmark
 * has no such page, so it goes to its list instead.
 */
export function goBack(): void {
  if (canGoBack()) {
    window.history.back();
    return;
  }
  const page = recordPage();
  if (page) void navigateToPage(RECORD_PAGES[page]);
}

/**
 * Opens a dialog or side pane and resolves when it closes, so the opener can reload its data.
 * `title` overrides the default, e.g. "Edit …" when the dialog opens an existing record.
 */
export async function openDialog(name: DialogName, params?: Params, title?: string): Promise<void> {
  const size: DialogSize = DIALOGS[name];
  const options: Xrm.Navigation.NavigationOptions = {
    target: 2,
    position: size.position,
    title: title ?? size.title,
    width: { value: size.width, unit: size.width > 100 ? "px" : "%" },
  };
  if (size.height !== undefined) options.height = { value: size.height, unit: "%" };
  await requireXrm().Navigation.navigateTo(
    { pageType: "webresource", webresourceName: `${PREFIX}${name}.html`, data: encode(params) },
    options,
  );
}

/**
 * Closes the dialog this page is shown in. Dataverse has no close API for web resource dialogs,
 * so this clicks the host's close button and falls back to window.close().
 */
export function closeDialog(): void {
  try {
    // Close the dialog that hosts this page: with nested dialogs (publish opened from the rule
    // editor) the first close button in the host is the outer dialog's, so start from our frame.
    const selector = 'button[data-id="dialogCloseIconButton"], button[title="Close"]';
    const own = window.frameElement?.closest('[role="dialog"]')?.querySelector<HTMLButtonElement>(selector);
    const all = window.parent.document.querySelectorAll<HTMLButtonElement>(selector);
    const button = own ?? all[all.length - 1];
    if (button) {
      button.click();
      return;
    }
  } catch {
    // Host document not reachable; fall through.
  }
  window.close();
}
