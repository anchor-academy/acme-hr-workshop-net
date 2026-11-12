"use client";
import { useEffect, useState } from "react";
import { api, LeaveRequest } from "@/lib/api";

export default function Leave() {
  const [rows, setRows] = useState<LeaveRequest[]>([]);
  const [error, setError] = useState("");
  useEffect(() => { api<LeaveRequest[]>("/leave").then(setRows).catch(e => setError(e.message)); }, []);
  return (
    <div className="card">
      <h2>Leave requests</h2>
      {error && <p className="error">{error}</p>}
      {/* TODO (Lab 2): a form to create a leave request (full or half day), and Approve / Reject buttons for Pending rows. */}
      <table><thead><tr><th>ID</th><th>Employee</th><th>From</th><th>To</th><th>Half day</th><th>Status</th><th>Reason</th></tr></thead>
        <tbody>{rows.map(r => <tr key={r.id}><td>{r.id}</td><td>{r.employeeId}</td><td>{r.startDate}</td><td>{r.endDate}</td><td>{r.halfDay}</td><td>{r.status}</td><td>{r.reason}</td></tr>)}</tbody>
      </table>
    </div>
  );
}
