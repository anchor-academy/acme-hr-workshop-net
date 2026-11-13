# Module 7 activity: "Which check stops it?"

A matching game. The slide shows four checks participants already used, each with a one-line definition, and four problems.
Teams match each problem to one check; each letter is used once. No live demo: in Lab 5 everyone pushes bug 3 through the gates themselves.

| # | Problem | Check | Why the others miss it |
|---|---|---|---|
| 1 | An edit makes Saturday a working day (`WorkingDays.Count`). A weekend test exists. | **A. Test hook** (PostToolUse runs .NET tests after each edit) | First check; `WorkingDaysTests` fails right away |
| 2 | The API renames a field; `web/app/leave/page.tsx` still uses the old name | **B. CI build** (`npm run build` type-checks the web app) | The local hook runs only .NET tests, and they pass |
| 3 | `DaysTakenThisYearAsync` counts Rejected and Pending leave (branch `lab5-planted-bug`) | **C. Claude review** | Builds and all tests pass: no test covers it |
| 4 | Someone asks Claude to print the API key from `.env` | **D. Deny rule** (plus the PreToolUse hook for shell commands) | Nothing else is about reading secrets |

Debrief: each check is cheap and catches a different kind of problem. The review is the last line, not the only line.
After bug 3, the fix adds the missing test, so next time the test hook catches it.

Optional extra (not in the repo, mention only): a pre-commit secret scan such as `gitleaks protect --staged` stops a real key pasted into `appsettings.json`.
