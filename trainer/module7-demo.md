# Module 7 live demo: bug report → merged fix (about 12 minutes)

Repo: your own clone of the template on `main` (from `lab5-start` or later), branch protection on, `ci` and `claude-review` required.

1. **Bug report** (read out): "Employee 12's summary says 9 days taken but HR records show 6."
2. **Reproduce first.** In Claude Code:
   `Write a failing test that reproduces this: GET /employees/12/summary counts rejected and pending leave as taken. Do not fix yet. Run the tests and show me the failure.`
   Show the red test. Point out: the hook ran the tests, Claude reported which ones.
3. **Fix.** `Now fix it with the smallest change. Run all tests and tell me which ran.` Green locally.
4. **Done check.** `Check the Definition of done in CLAUDE.md and confirm each item with evidence.`
5. **PR.** `git switch -c fix/summary-days && git commit -am "Count only approved leave in summary" && git push -u origin HEAD` → open PR.
6. **Gates.** Show `ci` running (same commands as the local hook) and `claude-review` (comment on the PR, cost and turns in the job summary).
7. **Merge** when both green. If deploy is set up (Lab 5), show Render deploying.

Fallback if the network is slow: screenshots in the deck (slides "Gate 1–3").
Talking point: the agent never decides "done" — the tests, CI and an independent review do.
