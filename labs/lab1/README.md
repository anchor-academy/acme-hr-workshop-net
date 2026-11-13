# Lab 1 — Code Generation with Claude Code: the leave API (35 min)

**Goal:** build the leave feature of Acme HR with Claude Code, under team rules that hold on their own.

**Branch:** `git checkout lab1-start` · Solution: `lab2-start` contains the finished Lab 1.

## Steps
1. Read `CLAUDE.md`, `.claude/rules/` and `.claude/settings.json`. Which lines are instructions, and which are enforced?
2. Start `claude` (accept the trust dialog the first time), switch to plan mode (Shift+Tab), paste the **Lab 1 plan prompt** from [PROMPTS.md](../../PROMPTS.md). Review the plan and ask for one change.
3. Approve and build (paste **Build it**). Watch the test hook run after each edit.
4. Paste **Try to break the rules**. Three layers should stop it: the CLAUDE.md instruction (Claude may refuse on its own), the `Read` deny rule, and the PreToolUse hook `scripts/block-secrets.js` for shell commands. Only the last two are enforced.
5. Run the API and the web app. Sign in as Farah: create a 2-day request (approved) and a 5-day request (pending). Sign out, sign in as Aina (her manager) and approve it.

## Done when
- [ ] The plan was reviewed before any code
- [ ] All tests pass, including the weekend test that failed
- [ ] The Leave page creates requests and shows their status
- [ ] Farah cannot approve her own request (403); Aina can
- [ ] Reading `.env` was blocked by the deny rule and by the hook

## Stuck?
- Hook keeps failing: read the test names it prints; ask Claude to fix those tests first.
- API not reachable from the web app: is the API running on port 5080? Is `web/.env.local` set?

## Debrief
Which of these controls would you add to your own repos on Monday?
