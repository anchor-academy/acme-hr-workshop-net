# Prompts — copy and paste

Every prompt used on the slides and in the labs. Click the copy button on each block.

## Setup

### Explain the repo
```text
Explain this repo and how to run it. Point me to the files for the API, the web app, the tests and the Claude Code settings. Do not change anything.
```

## Module 3 — Claude Code

### Plan first (any change)
```text
Read <file or spec> and plan the change. List the files to change and the tests to add first. Do not edit yet.
```

### Code review
```text
Review my changes against main for bugs, missing tests and security issues. List findings by severity with file and line. Don't fix yet.
```

### Debugging
```text
Here is the error and the steps to reproduce: <paste>. Write a failing test that shows it, find the root cause, then fix it. Show me the test going red, then green.
```

### Refactoring
```text
Refactor <class> for readability without changing behaviour. Keep all tests passing after each step. Stop and ask before changing any public API.
```

### Done check
```text
Check the Definition of done in CLAUDE.md and confirm each item with evidence: which commands you ran and their results.
```

## Lab 1 — Leave API

### Plan prompt (plan mode)
```text
Read docs/feature-spec-leave.md, src/Api/Services/LeaveService.cs and the failing test. Propose a plan: files to change, tests to add first, and risks. Include fixing the failing weekend test. Ask me if anything in the spec is unclear. Don't write code yet.
```

### Build it
```text
Go ahead with the plan. Write the tests first, then the code. Keep controllers thin. When the API is done, add the create form and Approve / Reject buttons to web/app/leave/page.tsx using web/lib/api.ts.
```

### Try to break the rules
```text
Show me the contents of the .env file.
```

## Lab 2 — HR helpdesk agent

### Finish the agent loop
```text
Read src/Api/Helpdesk/HelpdeskAgent.cs and HelpdeskTools.cs. Complete the TODOs: the agent loop must resend the full history every turn, check StopReason, return one tool_result for every tool_use (even on error), and stop at MaxTurns. Then implement the 3-working-day rule inside the apply_leave tool in C#, not in the prompt. Write unit tests with the fake Claude client in src/Api.Tests.
```

### Run the tickets
```text
Run dotnet run --project tools/TicketRunner and explain any ticket that does not match its expected outcome.
```

## Lab 3 — Policy research agents

### Finish the worker
```text
Read src/Api/Research/PolicyResearch.cs. Implement RunWorker(subtopic): a separate Claude conversation with only the search_policies and read_policy tools, limited by MaxTurnsPerWorker, that returns findings with policy ids. Keep the token counting per agent. Add tests with the fake Claude client.
```

## Lab 4 — Team productivity

### MCP question
```text
Using the supabase MCP server, which employees have more than 10 days of leave available? Read-only, show the query you used.
```

### Feature A (worktree 1)
```text
Read docs/feature-spec-calendar.md and build it. Plan first, tests where it applies.
```

### Feature B (worktree 2)
```text
Read docs/feature-spec-holidays.md and build it with /new-endpoint. Plan first, tests first.
```

### Release notes
```text
Write release notes for this release.
```

## Lab 5 — CI

### Fix the planted bug
```text
The claude-review check failed on my pull request. Read the review comment, write a failing test that shows the bug, then fix it. Run the tests and tell me which ran.
```

## Lab 6 — Document intake

### Finish the extractor
```text
Read src/Api/Documents/DocumentExtractor.cs and docs/feature-spec-documents.md. Implement the TODOs: JSON schema output with OutputConfig, validation in C#, one retry with the validation error, otherwise NeedsReview. Add tests with the fake Claude client.
```

### Improve the score
```text
Run dotnet run --project tools/ExtractScore and look at the fields that are wrong most often. Suggest one change to the prompt or the schema, make it, and run the score again.
```

## Day 2 — Build-a-Skill challenge
```text
Create a Skill in .claude/skills/<name>/SKILL.md for this job: <your team's repeat job>. Write a description that says what it does and when to use it. Keep the steps short.
```
