---
paths:
  - "web/app/**"
---
# Web page rules
- Call the API only through `web/lib/api.ts`. No direct database access, no secrets in the browser.
- Show loading and error states on every page that fetches data.
