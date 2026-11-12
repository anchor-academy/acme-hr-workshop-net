# Acme HR (training project)

A small HR platform with AI, built step by step in the Claude For Architect Workshop.
All data is synthetic. Company: Acme HR Sdn Bhd.

## Stack
- `src/Api` — .NET 8 Web API (C#), EF Core. Postgres on Supabase when `SUPABASE_DB_URL` is set, else local SQLite.
- `src/Api.Tests` — xUnit tests (in-memory SQLite).
- `web/` — Next.js app (TypeScript). Calls the API through `web/lib/api.ts` only.
- Claude API via the official `Anthropic` NuGet package. Key from `ANTHROPIC_API_KEY`.

## Commands
- Build: `dotnet build`
- Test: `dotnet test`
- Run API: `dotnet run --project src/Api` (http://localhost:5080, Swagger at /swagger)
- Run web: `cd web && npm run dev` (http://localhost:3000)
- Format: `dotnet format`

## Conventions
- Controllers stay thin; logic lives in `src/Api/Services/`.
- Leave days are working days (Mon–Fri). Half day = 0.5. See @docs/policies-summary.md.
- Rules that must always hold (approval limits, money, access) are enforced in C#, never only in a prompt.
- Every bug fix starts with a failing test.
- Never read or print `.env` or any secret.

## Definition of done
- `dotnet build` and `dotnet test` pass with no new warnings.
- Say which tests you ran and the result.
- New endpoints have tests for the happy path and validation errors.
