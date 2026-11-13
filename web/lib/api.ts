// The only place the web app talks to the Acme HR API.
const BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5080";
const BASE_PATH = process.env.NEXT_PUBLIC_BASE_PATH ?? "";

export async function api<T>(path: string, init?: RequestInit): Promise<T> {
  const token = typeof window === "undefined" ? null : window.localStorage.getItem(TOKEN_KEY);
  const res = await fetch(`${BASE}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(init?.headers ?? {}),
    },
    cache: "no-store",
  });
  if (res.status === 401 && token && typeof window !== "undefined") {
    signOut(); // token expired: back to the sign-in page
    throw new Error("Please sign in again");
  }
  if (!res.ok) {
    let msg = `${res.status} ${res.statusText}`;
    try { const p = await res.json(); msg = p.detail ?? p.title ?? msg; } catch {}
    throw new Error(msg);
  }
  return res.status === 204 ? (undefined as T) : res.json();
}

export type Employee = { id: number; name: string; email: string; department: string; managerId: number | null; annualEntitlement: number; roles?: string[] };
export type LeaveRequest = { id: number; employeeId: number; startDate: string; endDate: string; halfDay: "None" | "AM" | "PM"; status: "Pending" | "Approved" | "Rejected"; reason: string };
export type Balance = { employeeId: number; entitlement: number; used: number; pending: number; available: number };

// Sign-in: email + password against the API's built-in ASP.NET Core Identity endpoints.
// The bearer token and the signed-in employee are kept in this browser only.
const TOKEN_KEY = "acme.token";
const ME_KEY = "acme.me";

export async function signIn(email: string, password: string): Promise<Employee> {
  const res = await fetch(`${BASE}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });
  if (!res.ok) throw new Error(res.status === 401 ? "Wrong email or password" : `${res.status} ${res.statusText}`);
  const { accessToken } = await res.json();
  window.localStorage.setItem(TOKEN_KEY, accessToken);
  const me = await api<Employee>("/me");
  window.localStorage.setItem(ME_KEY, JSON.stringify(me));
  return me;
}

export function signOut() {
  window.localStorage.removeItem(TOKEN_KEY);
  window.localStorage.removeItem(ME_KEY);
  window.location.href = `${BASE_PATH}/login/`;
}

export function currentEmployee(): Employee | null {
  if (typeof window === "undefined") return null;
  const raw = window.localStorage.getItem(ME_KEY);
  return raw ? (JSON.parse(raw) as Employee) : null;
}

export function currentEmployeeId(): number {
  return currentEmployee()?.id ?? 0;
}
