#!/usr/bin/env bash
# PreToolUse hook for Bash: block commands that mention secret files.
# A Read deny rule stops `cat .env`, but not every command (for example `grep -r` over the folder).
# This hook is a second layer. Exit 2 blocks the command and tells Claude why.
cmd=$(python3 -c 'import json,sys; print(json.load(sys.stdin).get("tool_input",{}).get("command",""))' 2>/dev/null)
cmd=${cmd//.env.example/}   # the example file is fine
if echo "$cmd" | grep -Eq '(^|[^A-Za-z0-9_])\.env([^A-Za-z0-9_]|$)|secrets/|id_rsa|\.pem'; then
  echo "Blocked: this command touches a secret file. Use .env.example or ask a person." >&2
  exit 2
fi
exit 0
