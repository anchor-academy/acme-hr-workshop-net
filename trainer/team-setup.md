# Participant setup (send 3 days before)

Everyone builds their own copy of Acme HR, so everyone needs these accounts. Teams sit together and help each other, but nobody shares a repo. All free tiers. Sign in with GitHub everywhere.

| Account | Why | Free-tier notes |
|---|---|---|
| GitHub | Your repo from the template; Actions for CI | Repo must be **public** to require status checks on GitHub Free |
| Supabase | Postgres database (needed from Lab 4; SQLite before that) | Region **Singapore**. Use the pooler connection string (port 6543). Free projects pause after a week idle: open the dashboard the morning of Day 1 |
| Render | Hosts the API (Docker) | Free web service sleeps after 15 min idle; first request takes ~1 min |
| Vercel | Hosts the web app | Hobby plan; set Root Directory to `web` |

## Steps
1. Template repo → **Use this template** → tick **Include all branches** → create under your own account.
2. Supabase → New project (Singapore) → Project Settings → Database → copy the pooler connection string into `.env` as `SUPABASE_DB_URL`.
3. Clone, copy `.env.example` to `.env`, run `./check-setup.sh` (or `.ps1`). Health page must say "database connected".
4. Without Supabase the API runs on local SQLite, so nobody is blocked on Day 1 morning.

## On the day (trainer)
- API keys: one per participant (or per team, shared on paper), from an Anthropic Console workspace with a spend limit. Give out on paper or in the chat, never commit.
- Supabase access token (Lab 4 MCP): each participant creates one under Account → Access Tokens.
- Revoke all keys after Day 2.
