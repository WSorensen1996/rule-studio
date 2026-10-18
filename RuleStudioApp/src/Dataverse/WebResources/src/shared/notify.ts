// Short confirmations ("Draft saved") and the page status line (loading / error).
let toastTimer: number | undefined;

/** Shows a transient confirmation in the corner of the page. */
export function toast(message: string): void {
  let element = document.querySelector<HTMLElement>("#rsToast");
  if (!element) {
    element = document.createElement("div");
    element.id = "rsToast";
    element.className = "toast";
    element.setAttribute("role", "status");
    document.body.append(element);
  }
  element.textContent = message;
  element.hidden = false;
  window.clearTimeout(toastTimer);
  toastTimer = window.setTimeout(() => {
    if (element) element.hidden = true;
  }, 3500);
}

/** Writes to the page's status line (an element with role="status"). Empty text clears it. */
export function setStatus(element: HTMLElement, text: string, kind: "info" | "error" = "info"): void {
  element.textContent = text;
  element.classList.toggle("error", kind === "error");
}

/** Logs the technical detail and returns a message fit for the status line. */
export function describeError(action: string, error: unknown): string {
  console.error(`Rule Studio: ${action} failed`, error);
  return `Could not ${action}. Check your connection and access, then try Refresh.`;
}
