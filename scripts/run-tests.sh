#!/usr/bin/env bash
# PostToolUse hook: run the .NET tests after every edit.
# Exit 2 sends the failure back to Claude so it must fix it.
out=$(dotnet test --nologo -v q 2>&1)
if [ $? -ne 0 ]; then
  echo "Tests failing after this edit:" >&2
  echo "$out" | grep -E "Failed |error" | head -20 >&2
  exit 2
fi
exit 0
