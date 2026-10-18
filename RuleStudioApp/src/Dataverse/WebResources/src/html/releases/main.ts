// Releases page: history of rs_release rows, newest first. Opens the release detail pane and
// the publish and rollback dialogs.
import { rs_release_rs_status } from "../../../typings/XRM/OptionSets";
import { applyRole } from "../../shared/auth";
import { loadReleases, outcomePill, releaseStatusPill, rollbackButton, scopeText } from "../../shared/data/releases";
import { byId, h } from "../../shared/dom";
import { navigateToPage, openDialog, openRecord } from "../../shared/nav";
import { describeError, setStatus } from "../../shared/notify";
import { applyStandard } from "../../shared/standard";
import { initWebApi } from "../../shared/xrm";

import type { ReleaseSummary } from "../../shared/data/releases";
import type { DialogName, Params } from "../../shared/nav";

applyStandard();

const statusLine = byId("status");
let author = false;

async function openAndReload(name: DialogName, params?: Params): Promise<void> {
  try {
    await openDialog(name, params);
  } catch (error) {
    setStatus(statusLine, describeError("open the dialog", error), "error");
  }
  await refresh();
}

function row(release: ReleaseSummary): HTMLTableRowElement {
  const inspect = () => void openRecord("release-detail", { releaseId: release.id })
    .catch((error: unknown) => setStatus(statusLine, describeError("open the release details", error), "error"));
  const name = h("button", { className: "release-link", attrs: { type: "button" } },
    h("div", { className: "table-name" }, release.name),
    h("div", { className: "table-description" }, release.description || "No description"));
  name.addEventListener("click", inspect);
  const inspectButton = h("button", { className: "btn mini", attrs: { type: "button" } }, "Inspect");
  inspectButton.addEventListener("click", inspect);
  return h("tr", {},
    h("td", {}, name),
    h("td", {}, release.publishedOn, h("div", { className: "muted t11" }, `by ${release.publishedBy}`)),
    h("td", {}, scopeText(release)),
    h("td", {}, outcomePill(release.validation)),
    h("td", {}, releaseStatusPill(release.status)),
    h("td", {}, h("div", { className: "btn-group" }, inspectButton,
      ...rollbackButton(release, () => void openAndReload("rollback-release", { releaseId: release.id }), author))));
}

function showMetrics(releases: ReleaseSummary[]): void {
  byId("releaseCount").textContent = String(releases.length);
  byId("liveCount").textContent = String(releases.filter((r) => r.status === rs_release_rs_status.Live).length);
  byId("versionCount").textContent = String(releases.reduce((sum, r) => sum + r.versionCount, 0));
}

async function refresh(): Promise<void> {
  const button = byId<HTMLButtonElement>("refresh");
  button.disabled = true;
  setStatus(statusLine, "Loading releases…");
  try {
    const releases = await loadReleases();
    byId("releaseRows").replaceChildren(...releases.map(row));
    byId("emptyState").hidden = releases.length > 0;
    showMetrics(releases);
    setStatus(statusLine, "");
  } catch (error) {
    setStatus(statusLine, describeError("load the releases", error), "error");
  } finally {
    button.disabled = false;
  }
}

function start(): void {
  initWebApi();
  author = applyRole();
  byId("refresh").addEventListener("click", () => void refresh());
  byId("publish").addEventListener("click", () => void openAndReload("publish-release"));
  byId("openLibrary").addEventListener("click", () => {
    navigateToPage("rule-library").catch((error: unknown) => setStatus(statusLine, describeError("open the rule library", error), "error"));
  });
  void refresh();
}

start();
