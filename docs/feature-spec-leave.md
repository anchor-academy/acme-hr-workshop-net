# Feature spec: leave requests with half-day leave (Lab 1)

## Endpoints
- `POST /leave` — body: `startDate`, `endDate`, `halfDay` (`None` | `AM` | `PM`), `reason`.
  The employee is the signed-in user (`User.EmployeeId()`). An `employeeId` in the body is ignored.
- `POST /leave/{id}/approve` and `POST /leave/{id}/reject`.

## Rules
1. `endDate` must not be before `startDate`. Half day requires `startDate == endDate` and a working day.
2. Days are working days (Mon–Fri); a half day is 0.5.
3. Reject if the employee's available balance is less than the days requested.
4. Reject if it overlaps another Pending or Approved request of the same employee
   (two half days on the same date, one AM and one PM, do not overlap).
5. Up to 3 working days: status `Approved` immediately. More than 3: status `Pending` (manager approval).
6. Only `Pending` requests can be approved or rejected.
7. Only the requester's manager or HR may approve or reject, and never their own request
   (`AccessService.CanDecideLeaveAsync`). Anyone else gets 403.
8. Validation errors return 400 ProblemDetails with a clear message.

## Also
- Fix the known bug: leave spanning a weekend counts Saturday and Sunday (see the failing test).
- Web: the Leave page shows status and lets the user create a request.
