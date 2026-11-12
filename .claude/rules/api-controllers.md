---
paths:
  - "src/Api/Controllers/**/*.cs"
---
# Controller rules
- Keep controllers thin: validate input, call a service, return the result.
- Return ProblemDetails (`Problem(...)` / `ValidationProblem(...)`) for errors.
- Every new endpoint gets tests in `src/Api.Tests`.
