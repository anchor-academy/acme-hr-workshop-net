# Acme HR — Claude For Architect Workshop (.NET)

Anchor Academy · 18–19 November 2026

Your team builds **Acme HR**, a small HR platform with AI, one lab at a time, using **Claude Code** to build it and the **Claude API** inside it. All data is synthetic.

| Part | Tech | Hosted on |
|---|---|---|
| API | .NET 8 Web API (C#), EF Core | Render |
| Database, files | Supabase (Postgres, Storage) — or local SQLite while developing | Supabase |
| Web app | Next.js (TypeScript) in `web/` | Vercel |
| AI | Claude API (`Anthropic` NuGet package) + Claude Code | Anthropic |

## Setup (Day 1 morning, 15 minutes)

1. **One person per team:** click **Use this template** → tick **Include all branches** → create the repo under your account → add your teammates.
2. Everyone: `git clone` your team repo and `cd` into it.
3. Copy `.env.example` to `.env` and fill in `ANTHROPIC_API_KEY` (given on the day). Optional now, needed by Lab 4: create a Supabase project (region **Singapore**) and set `SUPABASE_DB_URL`. Without it the API uses a local SQLite file.
4. Run `./check-setup.sh` (macOS/Linux/Git Bash) or `./check-setup.ps1` (Windows PowerShell).
5. Run it:
   - API: `dotnet run --project src/Api` → http://localhost:5080/health should say **database connected**
   - Web: `cd web && cp .env.example .env.local && npm install && npm run dev` → http://localhost:3000
6. Start Claude Code in the repo (`claude`) and **accept the trust dialog**. Until you do, the team allow rules in `.claude/settings.json` are ignored and Claude asks before every `dotnet` command. Then paste **Explain the repo** from [PROMPTS.md](PROMPTS.md).

## Labs (build order)

Each lab starts from the previous lab's finished code. Behind? Check out the next start branch and keep going.

| Order | Lab | Branch | Adds to Acme HR |
|---|---|---|---|
| 1 | [Lab 2 — Code Generation with Claude Code](labs/lab2/README.md) | `lab2-start` | Leave API: requests, half-day leave, approvals |
| 2 | [Lab 1 — Customer Support Resolution Agent](labs/lab1/README.md) | `lab1-start` | HR helpdesk agent (Claude API + tools) |
| 3 | [Lab 3 — Multi-Agent Research System](labs/lab3/README.md) | `lab3-start` | Policy research agents with sources |
| 4 | [Lab 4 — Developer Productivity with Claude](labs/lab4/README.md) | `lab4-start` | Skill, Supabase MCP, two features in parallel |
| 5 | [Lab 5 — Claude Code for CI](labs/lab5/README.md) | `lab5-start` | CI gates + Claude review, deploy live |
| 6 | [Lab 6 — Structured Data Extraction](labs/lab6/README.md) | `lab6-start` | Document intake with HR review |

**All prompts:** [PROMPTS.md](PROMPTS.md) — copy and paste, no typing.

**Starter templates for your own repos:** [templates/ on the `final` branch](https://github.com/anchor-academy/acme-hr-workshop-net/tree/final/templates)

## Rules of the room
- Synthetic data only. Never paste company code or data into the labs.
- Never commit `.env`. Claude Code is configured not to read it.
- Stuck for 2 minutes? Red sticky note on your laptop, and we come to you.
