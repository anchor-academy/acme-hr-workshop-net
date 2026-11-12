# Lab 4 — Developer Productivity with Claude (35 min)

**Goal:** make Claude Code a team tool: a Skill, a read-only MCP connection to your data, and two features built in parallel.

**Branch:** `git checkout lab4-start` · Solution: `lab5-start`.

## Before you start
- Your team's Supabase project must be set in `.env` (`SUPABASE_DB_URL`) and the API run once against it (it creates and seeds the tables).
- Create a Supabase personal access token (Account → Access Tokens) and set `SUPABASE_ACCESS_TOKEN` and `SUPABASE_PROJECT_REF` in your shell before starting `claude`.

## Steps
1. Start `claude`, run `/mcp`: `supabase` should be connected (read-only). Paste **MCP question**.
2. Run `./make-worktrees.sh` (or `.ps1`). Open two terminals:
   - `../acme-wt-calendar` → paste **Feature A** (leave calendar page)
   - `../acme-wt-holidays` → paste **Feature B** (public holidays endpoint)
3. Merge both branches into your lab branch. Run `dotnet test` and `npm run build` in `web/`.
4. Paste **Release notes**. Check Claude used the `release-notes` Skill.

## Done when
- [ ] MCP answers from your live Supabase data
- [ ] Both features merged with tests
- [ ] Release notes follow the Skill's format

## Debrief
What in your codebase would a Skill capture, and what would an MCP server connect?
