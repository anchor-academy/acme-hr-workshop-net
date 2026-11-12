"use client";
import { useState } from "react";
import { api } from "@/lib/api";

type Usage = { agent: string; turns: number; inputTokens: number; outputTokens: number };
type Result = { answer: string; citationsValid: boolean; uncitedSentences: string[]; agents: Usage[]; totalTokens: number; stoppedBy: string };

export default function Policies() {
  const [q, setQ] = useState("Can I carry forward leave and take it as half days?");
  const [r, setR] = useState<Result | null>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  async function ask(e: React.FormEvent) {
    e.preventDefault(); setBusy(true); setError(""); setR(null);
    try { setR(await api<Result>("/research", { method: "POST", body: JSON.stringify({ question: q }) })); }
    catch (err) { setError((err as Error).message); } finally { setBusy(false); }
  }
  return (
    <>
      <form className="card" onSubmit={ask}>
        <h2>Policy assistant</h2>
        <input style={{ width: "80%" }} value={q} onChange={e => setQ(e.target.value)} /> <button disabled={busy}>{busy ? "Researching…" : "Ask"}</button>
        {error && <p className="error">{error}</p>}
      </form>
      {r && (
        <>
          <div className="card"><p>{r.answer}</p><p><b>Sources check:</b> {r.citationsValid ? "every sentence cites a policy" : `uncited: ${r.uncitedSentences.join(" | ")}`}</p></div>
          <div className="card">
            <h3>Agents · stopped by {r.stoppedBy} · {r.totalTokens} tokens</h3>
            <table><thead><tr><th>Agent</th><th>Turns</th><th>In</th><th>Out</th></tr></thead>
              <tbody>{r.agents.map(a => <tr key={a.agent}><td>{a.agent}</td><td>{a.turns}</td><td>{a.inputTokens}</td><td>{a.outputTokens}</td></tr>)}</tbody></table>
          </div>
        </>
      )}
    </>
  );
}
