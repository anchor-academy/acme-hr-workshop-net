# Lab 4 — Developer Productivity with Claude (35 min)

**Goal:** make Claude Code a team tool: a Skill, a read-only MCP connection to your data, and two features built in parallel.

**Branch:** `git checkout lab4-start` · Solution: `lab5-start`.

## Before you start
- Create your own free Supabase project (region Singapore), set `SUPABASE_DB_URL` in `.env` and run the API once against it (it creates and seeds the tables).
- Create a Supabase personal access token (Account → Access Tokens) and set `SUPABASE_ACCESS_TOKEN` and `SUPABASE_PROJECT_REF` in your shell before starting `claude`.

## Steps
1. Start `claude`, run `/mcp`: `supabase` should be connected (read-only). Paste **MCP question**.
2. Run `./make-worktrees.sh` (or `.ps1`). Open two terminals:
   - `../acme-wt-calendar` → paste **Feature A** (leave calendar page)
   - `../acme-wt-holidays` → paste **Feature B** (public holidays endpoint)
   - In the calendar worktree run `cd web && npm install` first (each worktree is a fresh folder).
   - When a feature is done, paste **Commit the feature** in that worktree.
3. Back in the main folder: `git merge feature/leave-calendar feature/public-holidays`. Run `dotnet test` and `npm run build` in `web/`.
4. Paste **Release notes**. Check Claude used the `release-notes` Skill.

## Done when
- [ ] MCP answers from your live Supabase data
- [ ] Both features merged with tests
- [ ] Release notes follow the Skill's format

## Debrief
What in your codebase would a Skill capture, and what would an MCP server connect?
