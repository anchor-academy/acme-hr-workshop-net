"use client";
import { useEffect, useState } from "react";
import { api, currentEmployee, StoredDocument } from "@/lib/api";

// HR review queue: documents that failed the checks in C# after one retry.
export default function Review() {
  const [docs, setDocs] = useState<StoredDocument[]>([]);
  const [error, setError] = useState("");
  const isHr = (currentEmployee()?.roles ?? []).includes("HR");
  useEffect(() => { if (isHr) api<StoredDocument[]>("/documents?status=NeedsReview").then(setDocs).catch(e => setError(e.message)); }, [isHr]);
  if (!isHr) return <div className="card"><h2>Needs review</h2><p>HR only. Sign in as an HR user (for example Dina Ismail) to see the queue.</p></div>;

  return (
    <div className="card">
      <h2>Needs review ({docs.length})</h2>
      {error && <p className="error">{error}</p>}
      <table>
        <thead><tr><th>#</th><th>Employee</th><th>File</th><th>Kind</th><th>Problems</th><th>Received</th></tr></thead>
        <tbody>
          {docs.map(d => (
            <tr key={d.id}><td>{d.id}</td><td>{d.employeeId}</td><td>{d.fileName}</td><td>{d.kind}</td><td>{d.problems}</td><td>{new Date(d.createdAt).toLocaleString()}</td></tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
