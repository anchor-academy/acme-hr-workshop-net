"use client";
import { useEffect, useState } from "react";
import { api, Employee } from "@/lib/api";

export default function Employees() {
  const [rows, setRows] = useState<Employee[]>([]);
  const [error, setError] = useState("");
  useEffect(() => { api<Employee[]>("/employees").then(setRows).catch(e => setError(e.message)); }, []);
  return (
    <div className="card">
      <h2>Employees</h2>
      {error && <p className="error">{error}</p>}
      <table><thead><tr><th>ID</th><th>Name</th><th>Department</th><th>Entitlement</th></tr></thead>
        <tbody>{rows.map(e => <tr key={e.id}><td>{e.id}</td><td>{e.name}</td><td>{e.department}</td><td>{e.annualEntitlement}</td></tr>)}</tbody>
      </table>
    </div>
  );
}
