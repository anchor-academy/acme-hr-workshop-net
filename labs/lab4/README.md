# Lab 4 — Developer Productivity with Claude (30 min)

**Goal:** make Claude Code a team tool: a Skill, a read-only MCP connection to your data, and two features built in parallel.

**Branch:** `git checkout lab4-start` · Solution: `lab5-start`.

## Before you start
- Render → New → **Postgres**: name `acme-hr-db`, region **Singapore**, plan **Free**. When it is ready: Connect → copy the **External Database URL** into `.env` as `DATABASE_URL`. Run the API once (`dotnet run --project src/Api`): it creates and seeds the tables in your Render database.
- Render → Account Settings → **API Keys** → create one. Set it in your shell before starting `claude` (not in `.env`):
  `export RENDER_API_KEY=...` (macOS/Linux/Git Bash) or `$env:RENDER_API_KEY="..."` (PowerShell).
- The key can change your whole Render account, so `.claude/settings.json` **denies** every Render MCP tool that creates or updates something. Claude can only read and run read-only SQL.

## Steps
1. Start `claude` (approve the `render` server once), run `/mcp`: `render` should be connected. Paste **MCP question**.
2. Run `./make-worktrees.sh` (or `.ps1`). Open two terminals:
   - `../acme-wt-calendar` → paste **Feature A** (leave calendar page)
   - `../acme-wt-holidays` → paste **Feature B** (public holidays endpoint)
   - In the calendar worktree run `cd web && npm install` first (each worktree is a fresh folder).
   - When a feature is done, paste **Commit the feature** in that worktree.
3. Back in the main folder: `git merge feature/leave-calendar feature/public-holidays`. Run `dotnet test` and `npm run build` in `web/`.
4. Paste **Release notes**. Check Claude used the `release-notes` Skill.

## Done when
- [ ] MCP answers from your live Render Postgres data, with a read-only query
- [ ] Both features merged with tests
- [ ] Release notes follow the Skill's format

## Debrief
What in your codebase would a Skill capture, and what would an MCP server connect?
