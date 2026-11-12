# Runaway loop demo (Module 7, 3 minutes)

Shows that limits in code stop an agent that would otherwise loop forever.

## 1. In the app (no API key needed)
`src/Api.Tests/HelpdeskAgentTests.cs` → `Stops_at_turn_limit`: the fake Claude asks for a tool on every turn (20 scripted calls). The agent stops at `MaxTurns = 8` and returns stop reason `turn_limit` with a safe reply.
```bash
dotnet test --filter Stops_at_turn_limit -v n
```
Change `MaxTurns` to 50 and rerun: the test fails because the agent keeps going — that is the runaway.

## 2. Headless Claude Code (needs a key)
A task that cannot succeed, with and without a turn cap:
```bash
claude -p "Run 'dotnet test --filter DoesNotExist' until at least one test passes." \
  --output-format json --max-turns 3 --allowedTools "Bash(dotnet test *)" | jq '{subtype, num_turns, total_cost_usd}'
```
Expect `subtype: "error_max_turns"`. In CI the job's `timeout-minutes` and the API workspace spend limit are the next two limits.

## Recover a runaway
Cancel the workflow run (or `gh run cancel <id>`), check cost in the job summary and Console usage, add or lower `--max-turns`, and add a test so the task can actually finish.
