## Anomaly classification

| Anomaly | Test  | Classification              | Evidence                                                                 | Remaining unknown |
|---------|-------|------------------------------|---------------------------------------------------------------------------|--------------------|
| ANO-01  | TC-007 | Application defect           | Same request key sent twice produced two different booking IDs for the same patient/doctor/time, reproduced in 3 of 3 retries | Whether the idempotency key is checked at all by the service or only partially, and whether this could double-charge or double-notify a patient in production |
| ANO-02  | TC-008 | Application defect           | 3 of 10 concurrent final-slot requests produced an unhandled InvalidOperationException instead of a controlled failure, even though the database never over-booked the slot | Whether the root cause is a missing lock/transaction around the slot check, or an unhandled database-level constraint violation |
| ANO-03  | TC-010 | Test asset or fixture problem | Fails only in the full-suite run, passes 5 of 5 in isolation. Inspection shows all cancellation tests share one static appointment object with parallel execution enabled | Whether the production service also has a genuine repeat-cancellation bug that the shared-state test pollution is currently masking |
| ANO-04  | TC-011 | Application defect           | An authenticated user cancelling another user's appointment received 204 instead of 403, reproduced with two different user pairs | Whether the missing authorisation check is isolated to this one endpoint or affects other appointment-modifying endpoints too |
| ANO-05  | TC-015, TC-016 | Environment/dependency incident | DNS resolution for sms-sandbox.test fails from both staging and the environment specialist's independent diagnostic container, matching the provider's own outage notice | How long the sandbox will remain down, and whether the SMS adapter code itself behaves correctly once the dependency is reachable again |

## Issues filed

| Anomaly | Test  | Classification              | GitHub Issue |
|---------|-------|------------------------------|--------------|
| ANO-04  | TC-011 | Application defect           | https://github.com/dhonburi/Week6AppointmentBooking/issues/1 |
| ANO-03  | TC-010 | Test asset or fixture problem | https://github.com/dhonburi/Week6AppointmentBooking/issues/2 |

## Activity 5 - Confirmation, regression and workflow status (AB-1.4-RC2)

| Result | Test | Type | Related |
|---|---|---|---|
| FR-01 | TC-007 | Confirmation | DEF-001 |
| FR-02 | TC-001 | Regression | DEF-001 |
| FR-03 | TC-017 | Regression | DEF-001 |
| FR-04 | TC-011 | Confirmation | DEF-002 (Issue #1, closed) |
| FR-05 | TC-015 | Previously blocked | ENV-001 |
| FR-06 | TC-016 | Previously blocked | DEF-003 (new issue filed) |
| FR-07 | TC-010 | Test-fixture verification | TEST-001 (Issue #2, closed) |

**DEF-001:** FR-01 proves the original retried-request failure is fixed. FR-03 reveals a
side effect: a genuinely different patient's booking for the same doctor and time is now
rejected as if it were a duplicate, likely because the check keys on doctor and time
alone rather than patient identity or the request key. Decision: DEF-001 remains in
Retest, with a new linked issue opened for the TC-017 regression, since closing it now
would hide a real problem the fix introduced.

**ENV-001:** sandbox recovery (FR-05) only confirms the happy path works again. It says
nothing about resilience, and the first real failure-mode test after recovery (FR-06)
immediately found a new defect: a provider error rolls back the whole booking instead of
just recording the reminder failure.

**TEST-001:** giving each cancellation test its own fixture (FR-07) removed the
suite-only failure, meaning TC-010's result now reflects actual product behaviour rather
than test execution order. This also indirectly supports that the underlying service
logic was fine all along.

**Targeted regression scope for the duplicate-booking fix:** TC-001 (ordinary booking
still succeeds), TC-002 (genuine "no slot available" still distinguishable from a
duplicate rejection), TC-008 (concurrency and duplicate-detection code paths likely
overlap), and TC-017 (the specific case that regressed, direct evidence a corrected fix
resolves it).