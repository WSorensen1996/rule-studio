// Rollback release dialog. Params: releaseId. Shows what each rule goes back to and calls
// rs_RollbackRelease; the opener reloads when the dialog closes.
import { loadPlan } from "./plan";
import { rs_release_rs_status } from "../../../typings/XRM/OptionSets";
import { applyRole } from "../../shared/auth";
import { loadRelease } from "../../shared/data/releases";
import { byId, h, pill } from "../../shared/dom";
import { closeDialog, pageParams } from "../../shared/nav";
import { describeError, setStatus, toast } from "../../shared/notify";
import { ApiError, rollbackRelease } from "../../shared/releaseApi";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { PlanLine } from "./plan";

applyStandard();

const statusLine = byId("status");
const confirmButton = byId<HTMLButtonElement>("confirm");
const { releaseId = "" } = pageParams();

let author = false;
let ready = false;
let busy = false;
let releaseName = "";

function updateButton(): void {
  confirmButton.disabled = !author || !ready || busy;
  if (author) confirmButton.title = ready ? "" : "Only a live release whose rules no later release changed can be rolled back.";
}

function planLine(line: PlanLine): HTMLElement {
  const from = line.versionNumber === null ? "v?" : `v${line.versionNumber}`;
  let outcome: HTMLSpanElement;
  if (line.blocked) outcome = pill("Changed by a later release", "bad");
  else if (line.previousNumber === null) outcome = pill("Stops running", "warning");
  else outcome = pill(`Goes back to v${line.previousNumber}`, "live");
  return h("div", { className: "list-line" },
    h("div", { className: "grow" }, h("b", { className: "t12" }, line.ruleName), h("div", { className: "muted t11 mono" }, line.ruleKey || "—")),
    h("div", { className: "plan-pills" }, pill(from, "info"), outcome));
}

function showBlocked(lines: PlanLine[]): void {
  const blocked = lines.filter((line) => line.blocked).map((line) => line.ruleName);
  const notice = byId("blocked");
  notice.hidden = blocked.length === 0;
  notice.replaceChildren(h("strong", {}, "This release cannot be rolled back yet"),
    `A later release changed ${blocked.join(", ")}. Roll back the later release first.`);
}

async function load(): Promise<void> {
  if (!releaseId) {
    setStatus(statusLine, "Open the rollback from the Releases page.", "error");
    return;
  }
  setStatus(statusLine, "Loading release…");
  try {
    const [release, lines] = await Promise.all([loadRelease(releaseId), loadPlan(releaseId)]);
    releaseName = release.name;
    byId("relName").textContent = release.name;
    byId("relRules").replaceChildren(...(lines.length
      ? lines.map(planLine)
      : [h("p", { className: "muted t12" }, "No rule versions are linked to this release.")]));
    showBlocked(lines);
    const live = release.status === rs_release_rs_status.Live;
    ready = live && lines.length > 0 && !lines.some((line) => line.blocked);
    setStatus(statusLine, live ? "" : "Only a live release can be rolled back.", live ? "info" : "error");
  } catch (error) {
    setStatus(statusLine, describeError("load the release", error), "error");
  }
  updateButton();
}

async function rollback(): Promise<void> {
  busy = true;
  updateButton();
  setStatus(statusLine, "Rolling back…");
  try {
    await rollbackRelease(releaseId);
    toast(`Rolled back "${releaseName}".`);
    closeDialog();
  } catch (error) {
    setStatus(statusLine, error instanceof ApiError ? `Could not roll back the release. Dataverse said: ${error.message}` : describeError("roll back the release", error), "error");
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
  confirmButton.addEventListener("click", () => void rollback());
  void load();
}

start();
