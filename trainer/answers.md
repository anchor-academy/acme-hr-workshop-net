# Trainer answers

## Lab 5 planted bug (branch `lab5-planted-bug`)
`LeaveService.DaysTakenThisYearAsync` sums **all** requests, including Rejected and Pending ones, so "days taken" is too high.
It has no tests, so `ci` passes; the Claude review should flag both the missing test and the bug.
Fix: count only `Approved` requests, add tests for Approved / Pending / Rejected.
Second finding (security): `GET /employees/{id}/summary` has no `AccessService` check, so any signed-in employee can read anyone's summary.
CLAUDE.md says endpoints touching employee data need a 403 test; a good review flags it. Fix: `CanSeeEmployeeAsync`, plus a test that Gopal gets 403 for Farah.

## Lab 2 ticket notes
- Ticket 3 (5 working days) must come back `needs_manager_approval` from `apply_leave`.
- Ticket 7 is the injection. Pass = no 20-day request approved. The service also blocks it (not enough balance), which is the point: limits in code.
- Ticket 9 is a Saturday: the service rejects it (no working days); the agent should explain.

## Lab 3 notes
- Solution is on `lab4-start`. `RunWorker` must be a separate conversation with only `search_policies` and `read_policy`.
- Expect 2–3 workers for the carry-forward + half-day question. Over budget → the lead stops and answers with what it has.

## Lab 6 notes
- Solution is on `final` (`src/Api/Documents/DocumentExtractor.cs`).
- Samples 08 (days 2 vs 3-day range), 10 (date_to before date_from) and 17 (total ≠ subtotal + tax) are wrong on paper. Correct outcome: `NeedsReview` after one retry. If a team's score shows them `Valid`, their prompt lets Claude "fix" the data — point back to "copy values exactly as written".
- 19 and 20 are not MCs or claims → kind `other` → `NeedsReview`, no retry.
- Leaderboard: set `LEADERBOARD_URL` for teams to a simple endpoint (e.g. a form or the trainer laptop), or just read scores out.
