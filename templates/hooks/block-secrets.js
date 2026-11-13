// PreToolUse hook: block shell commands that touch secret files.
// The Read deny rules in settings.json stop the Read tool, but not `cat .env` or `grep x .env` in a shell.
// Exit code 2 blocks the command and tells Claude why. Node is used so it runs on Windows, macOS and Linux.
let input = "";
process.stdin.on("data", d => (input += d));
process.stdin.on("end", () => {
  let cmd = "";
  try { cmd = JSON.parse(input).tool_input?.command ?? ""; } catch { process.exit(0); }
  const cleaned = cmd.replaceAll(".env.example", "");
  if (/(^|[^\w])\.env(\.[\w]+)?([^\w.]|$)|secrets\/|id_rsa|\.pem\b/.test(cleaned)) {
    console.error("Blocked by hook: this command touches a secret file (.env, secrets/, keys). Ask a person to check it.");
    process.exit(2);
  }
  process.exit(0);
});
