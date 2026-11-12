"use client";
import { useState } from "react";
import { api, StoredDocument } from "@/lib/api";

export default function Documents() {
  const [fileName, setFileName] = useState("");
  const [text, setText] = useState("");
  const [result, setResult] = useState<StoredDocument | null>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");

  async function pickFile(e: React.ChangeEvent<HTMLInputElement>) {
    const f = e.target.files?.[0]; if (!f) return;
    setFileName(f.name); setText(await f.text());
  }

  async function submit(e: React.FormEvent) {
    e.preventDefault(); setBusy(true); setError(""); setResult(null);
    try {
      setResult(await api<StoredDocument>("/documents", { method: "POST", body: JSON.stringify({ fileName, text }) }));
    } catch (err) { setError((err as Error).message); } finally { setBusy(false); }
  }

  return (
    <>
      <div className="card">
        <h2>Submit an MC or expense claim</h2>
        <p>Choose a .txt file (samples in <code>seed/documents</code>) or paste the text.</p>
        <form onSubmit={submit}>
          <p><input type="file" accept=".txt,.md" onChange={pickFile} /></p>
          <p><textarea rows={10} style={{ width: "100%" }} value={text} onChange={e => setText(e.target.value)} placeholder="Document text" /></p>
          <button disabled={busy || !text.trim()}>{busy ? "Reading…" : "Submit"}</button>
        </form>
        {error && <p className="error">{error}</p>}
      </div>
      {result && (
        <div className="card">
          <h3>{result.status === "Valid" ? "Accepted" : "Sent to HR for review"}</h3>
          <p>Kind: {result.kind}{result.problems && <> · Problems: {result.problems}</>}</p>
          <pre style={{ whiteSpace: "pre-wrap" }}>{result.extractedJson && JSON.stringify(JSON.parse(result.extractedJson), null, 2)}</pre>
        </div>
      )}
    </>
  );
}
