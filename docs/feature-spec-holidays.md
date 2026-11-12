# Feature B: public holidays endpoint (Lab 4, worktree 2)

- `GET /holidays?state=SEL&year=2026` returns `[{ date, name, state }]`.
- States: `SEL` (Selangor), `KUL` (Kuala Lumpur), `PNG` (Penang), and `ALL` (national holidays only).
- Seed a small synthetic list in `src/Api/Data/Holidays.cs` (national + a few state ones). Training data, not an official calendar.
- Unknown state returns 400 ProblemDetails.
- Logic in `src/Api/Services/HolidayService.cs`, controller thin. Tests for: national holidays, state filter, unknown state.
- Touch only API files so it merges cleanly with Feature A.
