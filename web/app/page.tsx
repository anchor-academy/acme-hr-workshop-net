"use client";
import { useEffect, useState } from "react";
import { api, Balance, currentEmployeeId } from "@/lib/api";

export default function Home() {
  const [health, setHealth] = useState<string>("checking…");
  const [balance, setBalance] = useState<Balance | null>(null);

  useEffect(() => {
    api<{ database: string }>("/health").then(h => setHealth(h.database)).catch(e => setHealth(`API error: ${e.message}`));
    api<Balance>(`/employees/${currentEmployeeId()}/balance`).then(setBalance).catch(() => setBalance(null));
  }, []);

  return (
    <>
      <div className="card"><b>API:</b> {health}</div>
      {balance && (
        <div className="card">
          <h2>My leave</h2>
          <p>Entitlement {balance.entitlement} · Used {balance.used} · Pending {balance.pending} · <b>Available {balance.available}</b></p>
        </div>
      )}
    </>
  );
}
