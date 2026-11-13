# Acme HR setup check (Windows PowerShell)
$fail = $false
function Ok($m){ Write-Host "  OK   $m" -ForegroundColor Green }
function Bad($m){ Write-Host "  FAIL $m" -ForegroundColor Red; $script:fail = $true }
function Warn($m){ Write-Host "  WARN $m" -ForegroundColor Yellow }
Write-Host "Acme HR setup check"
if (Get-Command git -ErrorAction SilentlyContinue) { Ok "git" } else { Bad "git not found" }
if ((Get-Command dotnet -ErrorAction SilentlyContinue) -and (dotnet --list-sdks | Select-String '^8\.')) { Ok ".NET 8 SDK" } else { Bad ".NET 8 SDK not found" }
if (Get-Command node -ErrorAction SilentlyContinue) { $v = [int]((node -v).TrimStart('v').Split('.')[0]); if ($v -ge 20) { Ok "Node $(node -v)" } else { Bad "Node.js 20+ needed" } } else { Bad "Node.js not found" }
if (Get-Command claude -ErrorAction SilentlyContinue) { Ok "Claude Code" } else { Bad "Claude Code not found" }
if (Test-Path .env) {
  Ok ".env present"
  Get-Content .env | Where-Object { $_ -match '^\s*[^#].*=' } | ForEach-Object { $k,$v = $_ -split '=',2; [Environment]::SetEnvironmentVariable($k.Trim(), $v.Trim(), 'Process') }
} else { Bad ".env missing: copy .env.example to .env and fill it in" }
if ($env:ANTHROPIC_API_KEY) { Ok "ANTHROPIC_API_KEY set (not shown)" } else { Bad "ANTHROPIC_API_KEY not set in .env" }
if ($env:DATABASE_URL) { Ok "DATABASE_URL set (Render Postgres)" } else { Warn "DATABASE_URL empty: the API uses local SQLite (fine until Lab 4)" }
if ($env:ANTHROPIC_API_KEY) {
  try {
    $body = '{"model":"claude-haiku-5-5","max_tokens":1,"messages":[{"role":"user","content":"hi"}]}'
    Invoke-RestMethod -Uri https://api.anthropic.com/v1/messages -Method Post -Body $body -ContentType 'application/json' -Headers @{ 'x-api-key' = $env:ANTHROPIC_API_KEY; 'anthropic-version' = '2023-06-01' } | Out-Null
    Ok "Claude API call works"
  } catch { Bad "Claude API call failed: $($_.Exception.Message)" }
}
dotnet build -v q --nologo *> $null; if ($LASTEXITCODE -eq 0) { Ok "dotnet build" } else { Bad "dotnet build failed" }
if ($fail) { Write-Host "Fix the FAIL rows, then run again."; exit 1 } else { Write-Host "All good. Run: dotnet run --project src/Api   and   cd web; npm install; npm run dev" }
