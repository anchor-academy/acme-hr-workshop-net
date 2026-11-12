"use client";
import { useEffect, useState } from "react";
import { api, Balance, Employee, currentEmployeeId, setCurrentEmployeeId } from "@/lib/api";

export default function Home() {
  const [health, setHealth] = useState<string>("checking…");
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [me, setMe] = useState<number>(6);
  const [balance, setBalance] = useState<Balance | null>(null);

  useEffect(() => {
    setMe(currentEmployeeId());
    api<{ database: string }>("/health").then(h => setHealth(h.database)).catch(e => setHealth(`API error: ${e.message}`));
    api<Employee[]>("/employees").then(setEmployees).catch(() => {});
  }, []);
  useEffect(() => { api<Balance>(`/employees/${me}/balance`).then(setBalance).catch(() => setBalance(null)); }, [me]);

  return (
    <>
      <div className="card"><b>API:</b> {health}</div>
      <div className="card">
        <label>Demo login as:&nbsp;
          <select value={me} onChange={e => { const id = Number(e.target.value); setCurrentEmployeeId(id); setMe(id); }}>
            {employees.map(e => <option key={e.id} value={e.id}>{e.name} ({e.department})</option>)}
          </select>
        </label>
      </div>
      {balance && (
        <div className="card">
          <h2>My leave</h2>
          <p>Entitlement {balance.entitlement} · Used {balance.used} · Pending {balance.pending} · <b>Available {balance.available}</b></p>
        </div>
      )}
    </>
  );
}
