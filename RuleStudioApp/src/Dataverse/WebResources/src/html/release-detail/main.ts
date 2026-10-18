// Release details page. Params: releaseId. Shows the release, the rule versions it
// published (each opens the rule editor) and its validation result.
import { renderValidation } from "./validation";
import { isAuthor } from "../../shared/auth";
import {
  costLabel, loadRelease, loadReleasedVersions, outcomePill, releaseStatusPill, rollbackButton, versionStatusPill,
} from "../../shared/data/releases";
import { byId, h, pill } from "../../shared/dom";
import { openDialog, openRecord, pageParams } from "../../shared/nav";
import { describeError, setStatus } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { ReleasedVersion, ReleaseSummary } from "../../shared/data/releases";

applyStandard();

const statusLine = byId("status");
let author = false;

async function openVersion(version: ReleasedVersion): Promise<void> {
  if (!version.ruleId) return;
  try {
    await openRecord("rule-editor", { ruleId: version.ruleId, versionId: version.id });
  } catch (error) {
    setStatus(statusLine, describeError("open the rule editor", error), "error");
  }
}

function versionLine(version: ReleasedVersion): HTMLElement {
  const open = h("button", { className: "btn mini", attrs: { type: "button" } }, "Open");
  open.disabled = !version.ruleId;
  open.addEventListener("click", () => void openVersion(version));
  const number = version.versionNumber === null ? "v?" : `v${version.versionNumber}`;
  return h("div", { className: "list-line" },
    h("div", { className: "grow" },
      h("b", { className: "t12" }, version.ruleName),
      h("div", { className: "muted t11 mono" }, version.ruleKey || "—"),
      h("div", { className: "muted t11" }, costLabel(version.costLevel))),
    h("div", { className: "rel-pills" }, pill(number, "info"), versionStatusPill(version.status)),
    open);
}

function showHeader(release: ReleaseSummary): void {
  byId("relName").textContent = release.name;
  document.title = `${release.name} · Rule Studio`;
  byId("relState").replaceChildren(releaseStatusPill(release.status));
  byId("relDate").textContent = release.publishedOn;
  byId("relBy").textContent = release.publishedBy;
  byId("relOutcome").replaceChildren(outcomePill(release.validation));
  byId("relNote").textContent = release.description || "No description";
  const restores = byId("relRestores");
  restores.hidden = !release.rolledBackReleaseId;
  restores.textContent = release.rolledBackReleaseId ? `Restores ${release.rolledBackReleaseName}` : "";
}

async function load(releaseId: string): Promise<void> {
  try {
    const [release, versions] = await Promise.all([loadRelease(releaseId), loadReleasedVersions(releaseId)]);
    showHeader(release);
    byId("rollbackSlot").replaceChildren(...rollbackButton(release, () => void openRollback(releaseId), author));
    byId("relRules").replaceChildren(...(versions.length
      ? versions.map(versionLine)
      : [h("p", { className: "muted t12" }, "No rule versions are linked to this release.")]));
    renderValidation(byId("relValidation"), release.validation);
    byId("detail").hidden = false;
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the release", error), "error");
  }
}

async function openRollback(releaseId: string): Promise<void> {
  try {
    await openDialog("rollback-release", { releaseId });
  } catch (error) {
    setStatus(statusLine, describeError("open the rollback dialog", error), "error");
  }
  await load(releaseId);
}

function start(): void {
  initWebApi();
  author = isAuthor();
  const { releaseId } = pageParams();
  if (!releaseId) {
    setStatus(statusLine, "Open release details from the Releases page.", "error");
    return;
  }
  void load(releaseId);
}

start();
