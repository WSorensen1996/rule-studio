// Access to the Dataverse host. Web resources run inside an iframe of the model-driven app,
// so the client API lives on the parent window.
import { setApiUrl } from "@delegateas/xrmquery";

type XrmWindow = Window & { Xrm?: Xrm.XrmStatic };

/** The host's Xrm object, or undefined when the page is opened outside Dataverse. */
export function hostXrm(): Xrm.XrmStatic | undefined {
  try {
    return (window.parent as XrmWindow).Xrm ?? (window as XrmWindow).Xrm;
  } catch {
    // A cross-origin parent throws on access: we are not inside Dataverse.
    return undefined;
  }
}

export function inDataverse(): boolean {
  return hostXrm()?.Navigation !== undefined;
}

/** Points XrmQuery at this environment's Web API. Call once before the first query. */
export function initWebApi(): void {
  setApiUrl(`${window.location.origin}/api/data/v9.2/`);
}

/** Web API base URL, for the few calls XrmQuery does not cover (metadata). */
export function webApiUrl(): string {
  return `${window.location.origin}/api/data/v9.2/`;
}
