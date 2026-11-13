# Participant setup (send 3 days before)

Everyone builds their own copy of Acme HR, so everyone needs these accounts. Teams sit together and help each other, but nobody shares a repo. All free tiers. Sign in with GitHub everywhere.

| Account | Why | Free-tier notes |
|---|---|---|
| GitHub | Your repo from the template; Actions for CI | Repo must be **public** to require status checks on GitHub Free |
| Render | Postgres database (from Lab 4; SQLite before that) and one Docker web service for the API + web app (Lab 5) | Region **Singapore**. One free Postgres per workspace, expires after 30 days (fine for the course). Free web service sleeps after 15 min idle; first request takes ~1 min |
| Anthropic Console | Your own API key (USD 10 prepaid, auto-reload off) | See the preparation guide |

## Steps
1. Template repo → **Use this template** → tick **Include all branches** → create under your own account.
2. Clone, copy `.env.example` to `.env`, set `ANTHROPIC_API_KEY`, run `./check-setup.sh` (or `.ps1`). Health page must say "database connected".
3. The API runs on local SQLite until Lab 4, so nobody is blocked on Day 1 morning.

## On the day (trainer)
- Lab 4: each participant creates a free Render Postgres (Singapore) and a Render API key (Account Settings → API Keys), exported as `RENDER_API_KEY` in the shell. The repo denies every Render MCP tool that creates or updates resources.
- Lab 5: Render → New → Blueprint deploys one service from `render.yaml`; `DATABASE_URL` = the Postgres **Internal** Database URL.
- Render maintenance window 18 Nov 10:00 MYT (Day 1 morning): no Render steps are needed that morning.
- After Day 2: participants revoke their Render API key and their Anthropic key.
