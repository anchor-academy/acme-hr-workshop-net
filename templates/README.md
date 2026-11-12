# Starter templates

Copy these into your own repositories after the workshop. Each file is small on purpose: keep what fits, delete the rest.

| File | What it is | Where it goes |
|---|---|---|
| `CLAUDE.md.template` | Team project memory: commands, conventions, Definition of done | Repo root as `CLAUDE.md` |
| `rules/api.md`, `rules/frontend.md` | Path-specific rules that load only for matching files | `.claude/rules/` |
| `team-settings.json` | Shared permissions (deny secrets, allow build/test) and a test hook | `.claude/settings.json` |
| `hooks/run-tests.sh`, `hooks/run-tests.ps1` | PostToolUse hook: run tests after each edit, exit 2 on failure | `.claude/hooks/` |
| `hooks/block-secrets.sh` | PreToolUse hook: block shell commands that touch secret files | `.claude/hooks/` |
| `ci/claude-headless-verify.yml` | Independent Claude review as a required GitHub check, with turn, time and cost limits | `.github/workflows/` |
| `managed/managed-settings.example.json` + README | Company policy that users and repos cannot override | Deployed by IT (see README) |
| `telemetry/otel.env.example` + README | Claude Code usage metrics to your OpenTelemetry collector | Managed settings or each user's settings |

Rules that must always hold (approvals, money, access) go in code, CI or managed settings, never only in a prompt.
