# Lab 3 — Multi-Agent Research System: policy research (35 min, runs 3rd)

**Goal:** a lead agent splits a policy question; workers research it with their own context; the answer cites a policy for every claim.

**Branch:** `git checkout lab3-start` · Solution: `lab4-start`.

## Steps
1. Read `src/Api/Research/PolicyResearch.cs`.
2. With Claude Code (paste **Finish the worker**), implement `RunWorker(subtopic)`: its own conversation, only `search_policies` and `read_policy`, `MaxTurnsPerWorker`.
3. Check the limits in code: `MaxWorkers`, `MaxTurnsPerWorker`, token budget.
4. Run the API and the web app. On the **Policies** page ask: *"Can I carry forward leave and take it as half days?"*

## Done when
- [ ] Every claim cites a policy id (the validator passes)
- [ ] The run stops inside your limits
- [ ] Token use per agent is shown

## Debrief
When would one agent with more tools have been enough?
