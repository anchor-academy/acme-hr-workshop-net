// The only place the web app talks to the Acme HR API.
const RAW = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5080";
const BASE = RAW.includes("://") ? RAW : `https://${RAW}`; // Render passes a bare host name

export async function api<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    ...init,
    headers: { "Content-Type": "application/json", ...(init?.headers ?? {}) },
    cache: "no-store",
  });
  if (!res.ok) {
    let msg = `${res.status} ${res.statusText}`;
    try { const p = await res.json(); msg = p.detail ?? p.title ?? msg; } catch {}
    throw new Error(msg);
  }
  return res.status === 204 ? (undefined as T) : res.json();
}

export type Employee = { id: number; name: string; email: string; department: string; managerId: number | null; annualEntitlement: number };
export type LeaveRequest = { id: number; employeeId: number; startDate: string; endDate: string; halfDay: "None" | "AM" | "PM"; status: "Pending" | "Approved" | "Rejected"; reason: string };
export type Balance = { employeeId: number; entitlement: number; used: number; pending: number; available: number };

// Demo login: the training app has no real accounts. The chosen employee id is kept in the browser.
export function currentEmployeeId(): number {
  if (typeof window === "undefined") return 6;
  return Number(window.localStorage.getItem("acme.employeeId") ?? 6);
}
export function setCurrentEmployeeId(id: number) {
  window.localStorage.setItem("acme.employeeId", String(id));
}

export type StoredDocument = { id: number; employeeId: number; fileName: string; kind: string; extractedJson: string; status: "Valid" | "NeedsReview"; problems: string; createdAt: string };
