// Restore version dialog. Params: versionId. Copies the version into a new draft with
// rs_RestoreVersion. When the rule already has a draft, the author confirms replacing it; the old
// draft is kept in history as Discarded. The rule editor reloads on the new draft when this closes.
import { XrmQuery } from "@delegateas/xrmquery";

import { applyRole } from "../../shared/auth";
import { byId, h } from "../../shared/dom";
import { closeDialog, pageParams } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { ApiError, restoreVersion } from "../../shared/releaseApi";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

applyStandard();

const statusLine = byId("status");
const confirmButton = byId<HTMLButtonElement>("confirm");
const { versionId = "" } = pageParams();

let author = false;
let ready = false;
let busy = false;
let replaceDraft = false;
let versionNumber: number | null = null;

function updateButton(): void {
  confirmButton.disabled = !author || !ready || busy;
  if (author) confirmButton.title = ready ? "" : "The version could not be loaded, or it is already the draft.";
}

async function draftNumber(draftId: string): Promise<number | null> {
  const draft = await XrmQuery.retrieve((x) => x.rs_ruleversions, draftId).select((x) => [x.rs_versionnumber]).execute();
  return draft.rs_versionnumber;
}

async function load(): Promise<void> {
  if (!versionId) {
    setStatus(statusLine, "Open the restore from the rule editor's version history.", "error");
    return;
  }
  setStatus(statusLine, "Loading version…");
  try {
    const version = await XrmQuery.retrieve((x) => x.rs_ruleversions, versionId)
      .select((x) => [x.rs_versionnumber])
      .expand((x) => x.rs_RuleId, (r) => [r.rs_name, r.rs_draftversionid_guid])
      .execute();
    versionNumber = version.rs_versionnumber;
    byId("restoreTarget").textContent = `${version.rs_RuleId?.rs_name ?? "Unnamed rule"} v${versionNumber ?? "?"}`;
    const draftId = version.rs_RuleId?.rs_draftversionid_guid ?? null;
    replaceDraft = draftId !== null;
    const notice = byId("replaceNotice");
    notice.hidden = !replaceDraft;
    if (draftId) {
      const draft = await draftNumber(draftId);
      notice.replaceChildren(h("strong", {}, "This replaces the current draft"),
        `The rule already has draft v${draft ?? "?"}. Restoring replaces it; the old draft stays in the history as Discarded.`);
      confirmButton.textContent = "Replace draft and restore";
    }
    ready = true;
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the version", error), "error");
  }
  updateButton();
}

async function restore(): Promise<void> {
  busy = true;
  updateButton();
  setStatus(statusLine, "Restoring…");
  try {
    await restoreVersion(versionId, replaceDraft);
    toast(`Created a new draft from version ${versionNumber ?? "?"}. Review it before publishing.`);
    closeDialog();
  } catch (error) {
    setStatus(statusLine, error instanceof ApiError ? `Could not restore the version. Dataverse said: ${error.message}` : describeError("restore the version", error), "error");
  } finally {
    busy = false;
    updateButton();
  }
}

function start(): void {
  initWebApi();
  author = applyRole();
  updateButton();
  byId("cancel").addEventListener("click", closeDialog);
  confirmButton.addEventListener("click", () => void restore());
  void load();
}

start();
