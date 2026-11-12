# PostToolUse hook (Windows): run tests after every edit. Exit 2 sends the failure back to Claude.
# In settings.json use: "command": "powershell -NoProfile -File .claude/hooks/run-tests.ps1"
$out = dotnet test --nologo -v q 2>&1
if ($LASTEXITCODE -ne 0) {
  [Console]::Error.WriteLine("Tests failing after this edit:")
  $out | Select-String -Pattern "Failed |error" | Select-Object -First 20 | ForEach-Object { [Console]::Error.WriteLine($_) }
  exit 2
}
exit 0
