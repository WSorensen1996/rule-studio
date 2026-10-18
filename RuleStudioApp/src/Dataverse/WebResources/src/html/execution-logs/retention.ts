// Retention card: shows Rule Studio's scheduled bulk-delete jobs and (re)schedules them with
// rs_ConfigureLogRetention.
import { byId } from "../../shared/dom";
import { describeError } from "../../shared/notify";
import { ApiError, callAction } from "../../shared/releaseApi";
import { webApiUrl } from "../../shared/xrm";

const JOB_PREFIX = "Rule Studio log retention";

interface JobRow { name: string; "postponeuntil@OData.Community.Display.V1.FormattedValue"?: string }

async function loadJobs(): Promise<JobRow[]> {
  const filter = encodeURIComponent(`startswith(name,'${JOB_PREFIX}') and statecode ne 3`);
  const response = await fetch(`${webApiUrl()}asyncoperations?$select=name,postponeuntil&$filter=${filter}&$orderby=name`, {
    headers: { Accept: "application/json", Prefer: "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"" },
  });
  if (!response.ok) throw new ApiError(`Reading system jobs failed (${response.status}).`, response.status);
  return ((await response.json()) as { value: JobRow[] }).value;
}

async function showJobs(): Promise<void> {
  const target = byId("retJobs");
  try {
    const jobs = await loadJobs();
    target.textContent = jobs.length
      ? jobs.map((job) => `${job.name.replace(`${JOB_PREFIX}: `, "")} (next run ${job["postponeuntil@OData.Community.Display.V1.FormattedValue"] ?? "pending"})`).join(" · ")
      : "No retention job is scheduled: logs are never deleted.";
  } catch (error) {
    target.textContent = describeError("read the scheduled jobs", error);
  }
}

async function schedule(button: HTMLButtonElement): Promise<void> {
  button.disabled = true;
  const target = byId("retJobs");
  try {
    const output = await callAction("rs_ConfigureLogRetention", {
      Days: Number(byId<HTMLInputElement>("retDays").value),
      ErrorDays: Number(byId<HTMLInputElement>("retErrorDays").value),
    });
    target.textContent = typeof output.Summary === "string" ? output.Summary : "Scheduled.";
    await showJobs();
  } catch (error) {
    target.textContent = error instanceof ApiError ? `Could not schedule the jobs. Dataverse said: ${error.message}` : describeError("schedule the jobs", error);
  } finally {
    button.disabled = false;
  }
}

export function initRetention(author: boolean): void {
  const button = byId<HTMLButtonElement>("retSchedule");
  button.addEventListener("click", () => void schedule(button));
  button.disabled = !author;
  void showJobs();
}
