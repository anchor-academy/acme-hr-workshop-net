# Module 7 activity: "Which gate catches it?"

Four bugs, each caught by a different gate. Show each diff, teams vote, then demo.

| # | Bug | Caught by | Why the earlier gates miss it |
|---|---|---|---|
| 1 | `WorkingDays.Count` counts Saturday (change `DayOfWeek.Saturday` check) | **PostToolUse hook** (local) — `WorkingDaysTests` fail right after Claude's edit | — first gate |
| 2 | A hard-coded API key in `appsettings.json` | **Pre-commit** (secret scan, e.g. `gitleaks protect --staged`) | Tests still pass; the hook only runs tests |
| 3 | `web/app/leave/page.tsx` uses a field the API no longer returns (TypeScript error) | **CI** (`npm run build`) | The local hook runs only .NET tests |
| 4 | `DaysTakenThisYearAsync` counts Rejected and Pending leave (branch `lab5-planted-bug`) | **Claude review only** | Builds, type-checks and all tests pass: no test covers it |

Debrief: each gate is cheap and catches a different class. The review is the last line, not the only line. After bug 4, the fix adds the missing test, so next time a cheaper gate catches it.

Pre-commit snippet for bug 2 (`.git/hooks/pre-commit` or pre-commit framework):
```bash
#!/usr/bin/env bash
gitleaks protect --staged --redact || { echo "Secret found. Commit blocked."; exit 1; }
```
