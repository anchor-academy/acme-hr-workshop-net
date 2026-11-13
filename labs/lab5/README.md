# Lab 5 — Claude Code for Continuous Integration: gates, then go live (30 min)

**Goal:** CI, not the agent's own report, decides "done". Then Acme HR goes live.

**Branch:** `git checkout lab5-start` and push it to your team repo's `main` (see step 0) · Solution: `lab6-start`.

## Steps
0. Make `lab5-start` your `main`: `git checkout lab5-start && git push origin lab5-start:main --force` (your own team repo only).
1. GitHub → Settings → Secrets and variables → Actions: add `ANTHROPIC_API_KEY` and `RENDER_DEPLOY_HOOK`.
2. Render: New → **Blueprint** → your repo (it reads `render.yaml`: one free Docker service that runs the API and serves the web app at `/app`). When asked, set `DATABASE_URL` to your Lab 4 Postgres **Internal Database URL** and `ANTHROPIC_API_KEY`.
3. Render → your `acme-hr` service → Settings → **Deploy Hook**: copy the URL into the GitHub secret `RENDER_DEPLOY_HOOK`.
4. GitHub → Settings → Branches: protect `main`, require the checks **ci** and **claude-review** (they appear after the first run).
5. Open a pull request from `lab5-planted-bug` to `main`. Watch `ci` pass and `claude-review` fail. Read the review comment and the cost in the job summary.
6. Fix it locally: check out `lab5-planted-bug`, copy the review comment from the pull request, paste **Fix the planted bug** and replace the placeholder with the comment. Push, merge when both checks are green. Watch the deploy job call Render. The first deploy takes a few minutes; then open `https://<your-service>.onrender.com` and sign in.

## Done when
- [ ] The review blocked the bad pull request
- [ ] The fix includes a test and is merged
- [ ] Acme HR is live on Render: sign in at `https://<your-service>.onrender.com`

## Stuck?
- Can't require checks: the repo must be public on GitHub Free, and the workflows must have run once.
- claude-review skipped: secret missing or misnamed.
- First page load takes about a minute: the free service sleeps after 15 minutes idle. Open `/health` to wake it.
- Deploy fails on the database: use the **Internal** Database URL (same region, Singapore), not the External one.

## Debrief
What stops the review job from running away: turns, time, money?
