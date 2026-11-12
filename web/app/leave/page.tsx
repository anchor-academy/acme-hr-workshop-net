"use client";
import { useEffect, useState } from "react";
import { api, currentEmployee, LeaveRequest } from "@/lib/api";

export default function Leave() {
  const [rows, setRows] = useState<LeaveRequest[]>([]);
  const [error, setError] = useState("");
  const [form, setForm] = useState({ startDate: "", endDate: "", halfDay: "None", reason: "" });

  const me = currentEmployee();
  // The API decides who may approve (403 otherwise). The page only hides buttons that would fail.
  const canDecide = (r: LeaveRequest) => r.status === "Pending" && r.employeeId !== me?.id && (me?.roles ?? []).some(x => x === "Manager" || x === "HR");
  const load = () => api<LeaveRequest[]>("/leave").then(setRows).catch(e => setError(e.message));
  useEffect(() => { load(); }, []);

  async function submit(e: React.FormEvent) {
    e.preventDefault(); setError("");
    try {
      await api("/leave", { method: "POST", body: JSON.stringify({ ...form, endDate: form.halfDay === "None" ? form.endDate : form.startDate }) });
      setForm({ startDate: "", endDate: "", halfDay: "None", reason: "" }); load();
    } catch (err) { setError((err as Error).message); }
  }
  async function decide(id: number, action: "approve" | "reject") {
    setError("");
    try { await api(`/leave/${id}/${action}`, { method: "POST" }); load(); } catch (err) { setError((err as Error).message); }
  }

  return (
    <>
      <form className="card" onSubmit={submit}>
        <h2>New leave request</h2>
        <p>
          <label>From <input type="date" required value={form.startDate} onChange={e => setForm({ ...form, startDate: e.target.value })} /></label>{" "}
          {form.halfDay === "None" && <label>To <input type="date" required value={form.endDate} onChange={e => setForm({ ...form, endDate: e.target.value })} /></label>}{" "}
          <label>Half day <select value={form.halfDay} onChange={e => setForm({ ...form, halfDay: e.target.value })}><option>None</option><option>AM</option><option>PM</option></select></label>
        </p>
        <p><input placeholder="Reason" value={form.reason} onChange={e => setForm({ ...form, reason: e.target.value })} /> <button>Submit</button></p>
        {error && <p className="error">{error}</p>}
      </form>
      <div className="card">
        <h2>Leave requests</h2>
        <table><thead><tr><th>ID</th><th>Employee</th><th>From</th><th>To</th><th>Half day</th><th>Status</th><th>Reason</th><th></th></tr></thead>
          <tbody>{rows.map(r => <tr key={r.id}><td>{r.id}</td><td>{r.employeeId}</td><td>{r.startDate}</td><td>{r.endDate}</td><td>{r.halfDay}</td><td>{r.status}</td><td>{r.reason}</td>
            <td>{canDecide(r) && <><button type="button" onClick={() => decide(r.id, "approve")}>Approve</button> <button type="button" onClick={() => decide(r.id, "reject")}>Reject</button></>}</td></tr>)}</tbody>
        </table>
      </div>
    </>
  );
}
