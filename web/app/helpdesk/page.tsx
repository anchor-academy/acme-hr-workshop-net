"use client";
import { useState } from "react";
import { api } from "@/lib/api";

type Turn = { role: "user" | "assistant"; text: string };
type ToolCall = { name: string; input: string; output: string; isError: boolean };
type Reply = { reply: string; toolCalls: ToolCall[]; turns: number; stopReason: string; inputTokens: number; outputTokens: number };

export default function Helpdesk() {
  const [turns, setTurns] = useState<Turn[]>([]);
  const [text, setText] = useState("");
  const [last, setLast] = useState<Reply | null>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");

  async function send(e: React.FormEvent) {
    e.preventDefault(); if (!text.trim()) return;
    const next: Turn[] = [...turns, { role: "user", text }];
    setTurns(next); setText(""); setBusy(true); setError("");
    try {
      // The app keeps the history and sends all of it every time.
      const r = await api<Reply>("/helpdesk/chat", { method: "POST", body: JSON.stringify({ messages: next }) });
      setTurns([...next, { role: "assistant", text: r.reply }]); setLast(r);
    } catch (err) { setError((err as Error).message); } finally { setBusy(false); }
  }

  return (
    <>
      <div className="card">
        <h2>HR helpdesk</h2>
        {turns.map((t, i) => <p key={i}><b>{t.role === "user" ? "You" : "Acme HR"}:</b> {t.text}</p>)}
        {busy && <p>Thinking…</p>}
        {error && <p className="error">{error}</p>}
        <form onSubmit={send}><input style={{ width: "75%" }} value={text} onChange={e => setText(e.target.value)} placeholder="Ask about leave or policy" /> <button disabled={busy}>Send</button></form>
      </div>
      {last && (
        <div className="card">
          <h3>Last run</h3>
          <p>Turns {last.turns} · stop {last.stopReason} · tokens {last.inputTokens} in / {last.outputTokens} out</p>
          <ul>{last.toolCalls.map((c, i) => <li key={i}><code>{c.name}</code> {c.input} → <code>{c.output}</code>{c.isError ? " (error)" : ""}</li>)}</ul>
        </div>
      )}
    </>
  );
}
