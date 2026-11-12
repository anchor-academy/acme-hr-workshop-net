# Feature spec: document intake (Lab 6)

Employees submit medical certificates (MC) and expense claims. Claude reads the document and returns data in a fixed shape. C# checks the data. Anything that fails goes to a person.

## Flow
1. Employee submits a document on the Documents page (choose a .txt file or paste text).
2. `POST /documents` calls `DocumentExtractor.ExtractAsync(text)`.
3. Claude returns JSON that follows `DocumentExtractor.Schema` (structured outputs via `OutputConfig`).
4. `DocumentValidator.Check` runs the business rules in C#.
5. If checks fail: retry **once**, sending Claude its own answer and the exact problems.
6. Still failing, or kind is `other`: save as `NeedsReview`. Otherwise save as `Valid`.
7. HR sees `NeedsReview` items on the Review page (`GET /documents?status=NeedsReview`).

## Data
- Medical certificate: `employee_name`, `clinic`, `date_from`, `date_to`, `days`.
- Expense claim: `claim_no`, `vendor`, `date`, `currency`, `subtotal`, `tax`, `total`, `items[]` (`description`, `amount`).
- Dates `yyyy-MM-dd`. Amounts are numbers, no currency symbol.

## Rules (in C#, never only in the prompt)
- Required text fields are not empty.
- Dates parse as `yyyy-MM-dd`; `date_to` is not before `date_from`.
- MC `days` equals the number of calendar days from `date_from` to `date_to`, inclusive.
- Claim `total` = `subtotal` + `tax`; items add up to `subtotal`; currency is a 3-letter code.
- Claude must copy values as written. A document that is wrong on paper must end in `NeedsReview`, not be "fixed" by Claude.

## Limits
- Max 2 calls to Claude per document. Max 20,000 characters. `max_tokens` 1024.

## Scoring
`dotnet run --project tools/ExtractScore -- --team "<team>"` sends the 20 samples in `seed/documents/` to `POST /documents/extract` and compares with `expected.json`. Score = 70% field accuracy + 30% correct status.
Samples 08, 10 and 17 are wrong on paper and should end in `NeedsReview`. Samples 19 and 20 are not MCs or claims.
