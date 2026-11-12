# Feature A: leave calendar page (Lab 4, worktree 1)

- New page `web/app/calendar/page.tsx`, linked as **Calendar** in the nav.
- Shows the current month as a grid (Mon–Sun). Each day lists the names of employees on Approved leave; Pending in grey.
- Half days show "(AM)" or "(PM)".
- Data comes from the existing `GET /leave` and `GET /employees` via `web/lib/api.ts`. No API changes.
- Buttons for previous / next month.
- Touch only files under `web/` so it merges cleanly with Feature B.
