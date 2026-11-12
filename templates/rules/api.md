---
paths:
  - "src/Api/Controllers/**"
---
# API controller rules
- Controllers stay thin: validate input, call one service method, map the result.
- Return errors as ProblemDetails (`Problem(...)`), never a plain string.
- Every new endpoint gets a test for the happy path and one validation error.
