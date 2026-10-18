// Plain Web API reads for queries XrmQuery cannot express (nested $expand across lookups).
import { webApiUrl } from "../../shared/xrm";

export async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(`${webApiUrl()}${path}`, {
    headers: { Accept: "application/json", "OData-Version": "4.0", "OData-MaxVersion": "4.0" },
    credentials: "same-origin",
  });
  if (!response.ok) throw new Error(`Web API request failed (${response.status}): ${path}`);
  return (await response.json()) as T;
}
