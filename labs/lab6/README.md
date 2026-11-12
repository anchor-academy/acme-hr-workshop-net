# Lab 6 — Structured Data Extraction: document intake (30 min)

**Goal:** turn messy documents (medical certificates, expense claims) into checked data, and send doubtful ones to a person.

**Branch:** `git checkout lab6-start` · Solution: `final`.

## Steps
1. Read `src/Api/Documents/DocumentExtractor.cs` and `docs/feature-spec-documents.md`.
2. With Claude Code (paste **Finish the extractor**): JSON schema output, validation in C#, one retry with the error, otherwise `NeedsReview`.
3. Run the API, then `dotnet run --project tools/ExtractScore -- --team "<team name>"`.
4. Paste **Improve the score**, then run the score again.
5. On your live site, paste a document on the **Documents** page and find it on the **Review** page.

## Done when
- [ ] 20/20 documents return valid data or NeedsReview
- [ ] Accuracy printed (and on the leaderboard)
- [ ] It works on the live site

## Debrief
Which errors should a person review, and how would you route them?
