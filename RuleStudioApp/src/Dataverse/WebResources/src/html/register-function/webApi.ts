// Plain Web API calls: system Custom API tables (not in the XrmQuery typings) and the
// rs_customfunction create/update, which needs the server's error message back.
import { webApiUrl } from "../../shared/xrm";

const HEADERS = { Accept: "application/json", "OData-Version": "4.0", "OData-MaxVersion": "4.0" };

async function failure(response: Response, path: string): Promise<Error> {
  try {
    const body = (await response.json()) as { error?: { message?: string } };
    if (body.error?.message) return new Error(body.error.message);
  } catch {
    // Not JSON; fall through.
  }
  return new Error(`Web API request failed (${response.status}): ${path}`);
}

export async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(`${webApiUrl()}${path}`, { headers: HEADERS, credentials: "same-origin" });
  if (!response.ok) throw await failure(response, path);
  return (await response.json()) as T;
}

/** POST (create) or PATCH (update, never upsert) a row. Resolves with the row's id. */
export async function writeRow(method: "POST" | "PATCH", path: string, body: Record<string, unknown>): Promise<string> {
  const headers: Record<string, string> = { ...HEADERS, "Content-Type": "application/json" };
  if (method === "PATCH") headers["If-Match"] = "*";
  const response = await fetch(`${webApiUrl()}${path}`, {
    method, headers, credentials: "same-origin", body: JSON.stringify(body),
  });
  if (!response.ok) throw await failure(response, path);
  // A create answers 204 with the new row's URL in OData-EntityId: …/rs_customfunctions(<id>).
  const entityId = response.headers.get("OData-EntityId") ?? path;
  return /\(([0-9a-f-]{36})\)/i.exec(entityId)?.[1] ?? "";
}
