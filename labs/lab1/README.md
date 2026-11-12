# Lab 1 — Customer Support Resolution Agent: HR helpdesk (35 min, runs 2nd)

**Goal:** add an HR helpdesk agent to Acme HR using the Claude API, safe by code, not by prompt.

**Branch:** `git checkout lab1-start` (contains the finished Lab 2) · Solution: `lab3-start`.

## Steps
1. Read `src/Api/Helpdesk/HelpdeskAgent.cs` and `HelpdeskTools.cs`.
2. With Claude Code (paste **Finish the agent loop**), complete the loop: resend history, check `StopReason`, one `tool_result` per `tool_use`, stop at `MaxTurns`.
3. The 4 tools: `get_leave_balance`, `apply_leave`, `check_policy`, `escalate_to_hr`. Put the rule in `apply_leave`: more than 3 working days returns `needs_manager_approval` and creates a Pending request.
4. Run the API, then `dotnet run --project tools/TicketRunner`. Then try the **Helpdesk** page.

## Done when
- [ ] Every one of the 10 tickets gets an outcome
- [ ] The 5-day request returns `needs_manager_approval` from code
- [ ] Ticket 7 (prompt injection) is refused
- [ ] No run goes past `MaxTurns`

## Stuck?
- 401 / invalid key: `ANTHROPIC_API_KEY` missing in `.env`. Restart the API.
- "tool_use ids without tool_result": return one tool_result for every tool_use, even when the tool fails.
- Loops forever: you did not add Claude's own turn to the history.

## Debrief
Which rule did you put in the prompt, and which in code? Why?
