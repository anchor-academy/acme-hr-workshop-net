# Lab 2 — Code Generation with Claude Code: the leave API (35 min, runs 1st)

**Goal:** build the leave feature of Acme HR with Claude Code, under team rules that hold on their own.

**Branch:** `git checkout lab2-start` · Solution: `lab1-start` contains the finished Lab 2.

## Steps
1. Read `CLAUDE.md`, `.claude/rules/` and `.claude/settings.json`. Which lines are instructions, and which are enforced?
2. Start `claude`, switch to plan mode (Shift+Tab), paste the **Lab 2 plan prompt** from [PROMPTS.md](../../PROMPTS.md). Review the plan and ask for one change.
3. Approve and build (paste **Build it**). Watch the test hook run after each edit.
4. Paste **Try to break the rules**. See the deny rule stop Claude from reading `.env`.
5. Run the API and the web app. Create a 2-day request (approved) and a 5-day request (pending), then approve it.

## Done when
- [ ] The plan was reviewed before any code
- [ ] All tests pass, including the weekend test that failed
- [ ] The Leave page creates requests and shows their status
- [ ] Reading `.env` was blocked

## Stuck?
- Hook keeps failing: read the test names it prints; ask Claude to fix those tests first.
- API not reachable from the web app: is the API running on port 5080? Is `web/.env.local` set?

## Debrief
Which of these controls would you add to your own repos on Monday?
