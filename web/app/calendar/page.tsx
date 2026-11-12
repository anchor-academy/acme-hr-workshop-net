"use client";
import { useEffect, useMemo, useState } from "react";
import { api, Employee, LeaveRequest } from "@/lib/api";

const iso = (d: Date) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;

export default function Calendar() {
  const [month, setMonth] = useState(() => { const d = new Date(); return new Date(d.getFullYear(), d.getMonth(), 1); });
  const [leave, setLeave] = useState<LeaveRequest[]>([]);
  const [people, setPeople] = useState<Record<number, string>>({});
  const [error, setError] = useState("");

  useEffect(() => {
    api<LeaveRequest[]>("/leave").then(setLeave).catch(e => setError(e.message));
    api<Employee[]>("/employees").then(es => setPeople(Object.fromEntries(es.map(e => [e.id, e.name])))).catch(() => {});
  }, []);

  const days = useMemo(() => {
    const first = new Date(month); const offset = (first.getDay() + 6) % 7; // Monday first
    const start = new Date(first); start.setDate(1 - offset);
    return Array.from({ length: 42 }, (_, i) => { const d = new Date(start); d.setDate(start.getDate() + i); return d; });
  }, [month]);

  const onDay = (d: Date) => leave.filter(r => r.status !== "Rejected" && r.startDate <= iso(d) && iso(d) <= r.endDate);

  return (
    <div className="card">
      <h2>
        <button onClick={() => setMonth(new Date(month.getFullYear(), month.getMonth() - 1, 1))}>‹</button>{" "}
        {month.toLocaleString("en-MY", { month: "long", year: "numeric" })}{" "}
        <button onClick={() => setMonth(new Date(month.getFullYear(), month.getMonth() + 1, 1))}>›</button>
      </h2>
      {error && <p className="error">{error}</p>}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(7, 1fr)", gap: 4 }}>
        {["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"].map(d => <b key={d}>{d}</b>)}
        {days.map(d => (
          <div key={iso(d)} style={{ minHeight: 70, padding: 4, border: "1px solid #ccc", background: d.getMonth() === month.getMonth() ? "#fff" : "#f3f3f3" }}>
            <small>{d.getDate()}</small>
            {onDay(d).map(r => (
              <div key={r.id} style={{ fontSize: 12, color: r.status === "Pending" ? "#888" : "#1F1147" }}>
                {people[r.employeeId] ?? `#${r.employeeId}`}{r.halfDay !== "None" ? ` (${r.halfDay})` : ""}
              </div>
            ))}
          </div>
        ))}
      </div>
    </div>
  );
}
