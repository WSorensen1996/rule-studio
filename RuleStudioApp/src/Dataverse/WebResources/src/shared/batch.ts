// Runs several Web API changes as one $batch changeset: Dataverse applies all of them in one
// transaction, or none. Used where XrmQuery falls short (clearing a lookup needs a null bind).
import { webApiUrl } from "./xrm";

export interface ChangeRequest {
  method: "POST" | "PATCH" | "DELETE";
  /** Relative to the Web API root, e.g. "rs_conditions" or "rs_conditions(<id>)". */
  path: string;
  body?: Record<string, unknown>;
}

export class BatchError extends Error {
  constructor(message: string, readonly status: number) {
    super(message);
    this.name = "BatchError";
  }
}

function part(request: ChangeRequest, contentId: number): string {
  const lines = [
    "Content-Type: application/http",
    "Content-Transfer-Encoding: binary",
    `Content-ID: ${contentId}`,
    "",
    `${request.method} ${webApiUrl()}${request.path} HTTP/1.1`,
    "Content-Type: application/json",
  ];
  // An update must never recreate a row someone else deleted in the meantime.
  if (request.method === "PATCH") lines.push("If-Match: *");
  lines.push("", request.body ? JSON.stringify(request.body) : "");
  return lines.join("\r\n");
}

function errorMessage(text: string): string {
  const match = /"message"\s*:\s*"((?:[^"\\]|\\.)*)"/.exec(text);
  if (!match?.[1]) return "The change set was rejected.";
  try {
    return JSON.parse(`"${match[1]}"`) as string;
  } catch {
    return match[1];
  }
}

/** Sends the requests in order as one transaction. Rejects with BatchError when any of them fails. */
export async function executeChangeset(requests: ChangeRequest[]): Promise<void> {
  if (requests.length === 0) return;
  const batch = `batch_${crypto.randomUUID()}`;
  const changeset = `changeset_${crypto.randomUUID()}`;
  const parts = requests.map((request, index) => `--${changeset}\r\n${part(request, index + 1)}`);
  const body = [
    `--${batch}`,
    `Content-Type: multipart/mixed; boundary=${changeset}`,
    "",
    ...parts,
    `--${changeset}--`,
    `--${batch}--`,
    "",
  ].join("\r\n");
  const response = await fetch(`${webApiUrl()}$batch`, {
    method: "POST",
    credentials: "same-origin",
    headers: {
      "Content-Type": `multipart/mixed; boundary=${batch}`,
      Accept: "application/json",
      "OData-Version": "4.0",
      "OData-MaxVersion": "4.0",
    },
    body,
  });
  const text = await response.text();
  // A failed changeset still answers 200 for the batch; the failing part carries its own status.
  const failed = [...text.matchAll(/HTTP\/1\.1 (\d{3})/g)].map((m) => Number(m[1])).find((status) => status >= 400);
  if (!response.ok || failed) throw new BatchError(errorMessage(text), failed ?? response.status);
}

/** "/rs_rules(<id>)" for an @odata.bind, or null to clear the lookup. */
export function bindTo(entitySet: string, id: string | null | undefined): string | null {
  return id ? `/${entitySet}(${id})` : null;
}
