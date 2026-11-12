# Acme HR — Claude For Architect Workshop (.NET)

Anchor Academy · 18–19 November 2026

Your team builds **Acme HR**, a small HR platform with AI, one lab at a time, using **Claude Code** to build it and the **Claude API** inside it. All data is synthetic.

| Part | Tech | Hosted on |
|---|---|---|
| API | .NET 8 Web API (C#), EF Core | Render (one Docker service) |
| Web app | Next.js (TypeScript) in `web/`, served by the API at `/app` | Render (same service) |
| Database | Render Postgres — or local SQLite while developing | Render |
| AI | Claude API (`Anthropic` NuGet package) + Claude Code | Anthropic |

## Setup (Day 1 morning, 15 minutes)

Everyone builds their own copy of Acme HR. Your team sits together, helps each other and scores together.

1. Click **Use this template** → tick **Include all branches** → create the repo under your own GitHub account.
2. `git clone` your repo and `cd` into it.
3. Copy `.env.example` to `.env` and fill in `ANTHROPIC_API_KEY` (your own Console key, see the prep guide). Keep it in `.env` only: if it is exported in your shell, Claude Code bills it instead of your Claude plan. Not needed until Lab 4: your own free Render Postgres and `DATABASE_URL`. Without it the API uses a local SQLite file.
4. Run `./check-setup.sh` (macOS/Linux/Git Bash) or `./check-setup.ps1` (Windows PowerShell).
5. Run it:
   - API: `dotnet run --project src/Api` → http://localhost:5080/health should say **database connected**
   - Web: `cd web && cp .env.example .env.local && npm install && npm run dev` → http://localhost:3000
6. Sign in at http://localhost:3000 with a training account (below).
7. Start Claude Code in the repo (`claude`) and **accept the trust dialog**. Until you do, the team allow rules in `.claude/settings.json` are ignored and Claude asks before every `dotnet` command. Then paste **Explain the repo** from [PROMPTS.md](PROMPTS.md).

## Sign in (training accounts)

Every seeded employee can sign in with their work email and the shared training password **`Acme@2026`**
(set `SEED_USER_PASSWORD` in `.env` to change it before the first run). Use these four:

| Who | Email | Roles | Can do |
|---|---|---|---|
| Farah Tan | `farah.tan6@acme-hr.example` | Employee | Own leave and balance |
| Aina Lee | `aina.lee1@acme-hr.example` | Employee, Manager | Own leave + approve her team (Farah is in it) |
| Dina Ismail | `dina.ismail4@acme-hr.example` | Employee, Manager, HR | Everyone's leave, HR review |
| Gopal Ng | `gopal.ng7@acme-hr.example` | Employee | Another team: use him to check you get 403 |

The full list is on the Employees page. Five wrong passwords lock an account for 15 minutes.
In Swagger: call `POST /auth/login`, copy `accessToken`, click **Authorize**, paste it.
Training only: a real system never shares one password between users.

## Labs (build order)

Each lab starts from the previous lab's finished code. Behind? Check out the next start branch and keep going.

| Order | Lab | Branch | Adds to Acme HR |
|---|---|---|---|
| 1 | [Lab 1 — Code Generation with Claude Code](labs/lab1/README.md) | `lab1-start` | Leave API: requests, half-day leave, approvals |
| 2 | [Lab 2 — Customer Support Resolution Agent](labs/lab2/README.md) | `lab2-start` | HR helpdesk agent (Claude API + tools) |
| 3 | [Lab 3 — Multi-Agent Research System](labs/lab3/README.md) | `lab3-start` | Policy research agents with sources |
| 4 | [Lab 4 — Developer Productivity with Claude](labs/lab4/README.md) | `lab4-start` | Skill, Render MCP, two features in parallel |
| 5 | [Lab 5 — Claude Code for CI](labs/lab5/README.md) | `lab5-start` | CI gates + Claude review, deploy live |
| 6 | [Lab 6 — Structured Data Extraction](labs/lab6/README.md) | `lab6-start` | Document intake with HR review |

Finished system (all labs done): branch `final`.

**All prompts:** [PROMPTS.md](PROMPTS.md) — copy and paste, no typing.

**Starter templates for your own repos:** [templates/ on the `final` branch](https://github.com/anchor-academy/acme-hr-workshop-net/tree/final/templates)

## Rules of the room
- Synthetic data only. Never paste company code or data into the labs.
- Never commit `.env`. Claude Code is configured not to read it.
- Stuck for 2 minutes? Red sticky note on your laptop, and we come to you.
