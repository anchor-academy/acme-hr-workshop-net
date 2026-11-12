# Lab 6 — Structured Data Extraction: document intake (30 min)

**Goal:** turn MCs and expense claims into checked data. Claude gives the shape; C# decides if it is right; a person reviews the rest.

**Branch:** `git checkout lab6-start` · Solution: `final`. Prompts: [PROMPTS.md](../../PROMPTS.md#lab-6--document-intake).

## Steps
1. Read `docs/feature-spec-documents.md`, `src/Api/Documents/DocumentExtractor.cs` and `DocumentValidator.cs`.
2. In Claude Code, paste **Finish the extractor**. Review the plan first (Shift+Tab for plan mode).
3. Run `dotnet test`. The extractor tests that fail at the start must pass.
4. Start the API, then run `dotnet run --project tools/ExtractScore -- --team "<your team>"`. Read which fields are wrong.
5. Paste **Improve the score** once. Run the scorer again.
6. Push to `main` (through a pull request, the gates from Lab 5 still apply). On the live site, submit one sample on **Documents** and find it on **Review** if it failed.

## Done when
- [ ] 20/20 documents end as `Valid` or `NeedsReview` (no errors)
- [ ] Samples 08, 10 and 17 end in `NeedsReview`, not "fixed" by Claude
- [ ] Your score is on the leaderboard
- [ ] It works on the live site

## Stuck?
- HTTP 500 on every document: `ANTHROPIC_API_KEY` is not set in the terminal running the API.
- `no such table: Documents`: restart the API; it adds missing tables at start-up.
- Claude "fixes" the wrong totals: the prompt must say copy values as written; the check is in C#.

## Debrief
Which errors should a person review, and which can code reject on its own?
