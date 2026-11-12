---
paths:
  - "web/app/**"
---
# Web page rules
- Call the API only through `web/lib/api.ts`. Never call the database from the web app.
- Keep pages simple: one page, one job. Show loading and error states.
