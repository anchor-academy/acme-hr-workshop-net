# Lab 5 — Claude Code for Continuous Integration: gates, then go live (35 min)

**Goal:** CI, not the agent's own report, decides "done". Then Acme HR goes live.

**Branch:** `git checkout lab5-start` and push it to your team repo's `main` (see step 0) · Solution: `lab6-start`.

## Steps
0. Make `lab5-start` your `main`: `git checkout lab5-start && git push origin lab5-start:main --force` (your own team repo only).
1. GitHub → Settings → Secrets and variables → Actions: add `ANTHROPIC_API_KEY` and `RENDER_DEPLOY_HOOK`.
2. Render: New → Blueprint → your repo (uses `render.yaml`). Set `SUPABASE_DB_URL` and `ANTHROPIC_API_KEY` in Render. Copy the **Deploy Hook** URL into the GitHub secret.
3. Vercel: Add New Project → your repo → Root Directory `web` → env `NEXT_PUBLIC_API_URL` = your Render URL. Add the Vercel URL to `AllowedOrigins` in Render.
4. GitHub → Settings → Branches: protect `main`, require the checks **ci** and **claude-review** (they appear after the first run).
5. Open a pull request from `lab5-planted-bug` to `main`. Watch `ci` pass and `claude-review` fail. Read the review comment and the cost in the job summary.
6. Fix it locally: check out `lab5-planted-bug`, copy the review comment from the pull request, paste **Fix the planted bug** and replace the placeholder with the comment. Push, merge when both checks are green. Watch Render and Vercel deploy.

## Done when
- [ ] The review blocked the bad pull request
- [ ] The fix includes a test and is merged
- [ ] Acme HR is live: web on Vercel calling the API on Render

## Stuck?
- Can't require checks: the repo must be public on GitHub Free, and the workflows must have run once.
- claude-review skipped: secret missing or misnamed.
- CORS error in the browser: add the Vercel URL to `AllowedOrigins` on Render.

## Debrief
What stops the review job from running away: turns, time, money?
