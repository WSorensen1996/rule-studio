// Author vs viewer. Security roles decide what Dataverse allows; this only keeps the UI honest
// by disabling controls a viewer cannot use, instead of letting them fail on save.
import { hostXrm } from "./xrm";

const AUTHOR_ROLES = new Set(["rule studio author", "system administrator", "system customizer"]);

export function isAuthor(): boolean {
  const roles = hostXrm()?.Utility.getGlobalContext().userSettings.roles;
  if (!roles) return false;
  let author = false;
  roles.forEach((role) => {
    if (AUTHOR_ROLES.has(role.name?.toLowerCase() ?? "")) author = true;
  });
  return author;
}

/** Disables every element marked data-author when the user is not an author. Returns isAuthor(). */
export function applyRole(root: ParentNode = document): boolean {
  const author = isAuthor();
  if (!author) {
    document.body.classList.add("viewer");
    root.querySelectorAll<HTMLButtonElement | HTMLInputElement | HTMLSelectElement>("[data-author]").forEach((element) => {
      element.disabled = true;
      element.title = "Requires the Rule Studio Author role.";
    });
  }
  return author;
}
