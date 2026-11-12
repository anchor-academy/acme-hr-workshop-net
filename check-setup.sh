#!/usr/bin/env bash
# Acme HR setup check (macOS / Linux / WSL / Git Bash)
ok(){ printf "  \033[32mOK\033[0m   %s\n" "$1"; }
bad(){ printf "  \033[31mFAIL\033[0m %s\n" "$1"; FAIL=1; }
warn(){ printf "  \033[33mWARN\033[0m %s\n" "$1"; }
FAIL=0
echo "Acme HR setup check"
command -v git >/dev/null && ok "git $(git --version | cut -d' ' -f3)" || bad "git not found"
command -v dotnet >/dev/null && dotnet --list-sdks | grep -q '^8\.' && ok ".NET 8 SDK" || bad ".NET 8 SDK not found (https://dotnet.microsoft.com/download)"
command -v node >/dev/null && [ "$(node -v | cut -c2- | cut -d. -f1)" -ge 20 ] && ok "Node $(node -v)" || bad "Node.js 20+ not found"
command -v claude >/dev/null && ok "Claude Code $(claude --version 2>/dev/null | head -1)" || bad "Claude Code not found (https://code.claude.com)"
[ -f .env ] && ok ".env present" || bad ".env missing: copy .env.example to .env and fill it in"
set -a; [ -f .env ] && . ./.env; set +a
[ -n "$ANTHROPIC_API_KEY" ] && ok "ANTHROPIC_API_KEY set (not shown)" || bad "ANTHROPIC_API_KEY not set in .env"
[ -n "$SUPABASE_DB_URL" ] && ok "SUPABASE_DB_URL set" || warn "SUPABASE_DB_URL empty: the API will use local SQLite"
if [ -n "$ANTHROPIC_API_KEY" ]; then
  code=$(curl -s -o /dev/null -w "%{http_code}" https://api.anthropic.com/v1/messages -H "x-api-key: $ANTHROPIC_API_KEY" -H "anthropic-version: 2023-06-01" -H "content-type: application/json" -d '{"model":"claude-haiku-5-5","max_tokens":1,"messages":[{"role":"user","content":"hi"}]}')
  [ "$code" = "200" ] && ok "Claude API call works" || bad "Claude API call returned HTTP $code"
fi
dotnet build -v q --nologo >/dev/null 2>&1 && ok "dotnet build" || bad "dotnet build failed"
command -v gh >/dev/null && (gh auth status >/dev/null 2>&1 && ok "GitHub CLI signed in" || warn "GitHub CLI not signed in (optional)") || warn "GitHub CLI not installed (optional)"
[ $FAIL -eq 0 ] && echo "All good. Run: dotnet run --project src/Api   and   cd web && npm install && npm run dev" || { echo "Fix the FAIL rows, then run again."; exit 1; }
