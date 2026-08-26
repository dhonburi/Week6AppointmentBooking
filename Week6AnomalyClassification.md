## Anomaly classification

| Anomaly | Test  | Classification              | Evidence                                                                 | Remaining unknown |
|---------|-------|------------------------------|---------------------------------------------------------------------------|--------------------|
| ANO-01  | TC-007 | Application defect           | Same request key sent twice produced two different booking IDs for the same patient/doctor/time, reproduced in 3 of 3 retries | Whether the idempotency key is checked at all by the service or only partially, and whether this could double-charge or double-notify a patient in production |
| ANO-02  | TC-008 | Application defect           | 3 of 10 concurrent final-slot requests produced an unhandled InvalidOperationException instead of a controlled failure, even though the database never over-booked the slot | Whether the root cause is a missing lock/transaction around the slot check, or an unhandled database-level constraint violation |
| ANO-03  | TC-010 | Test asset or fixture problem | Fails only in the full-suite run, passes 5 of 5 in isolation; inspection shows all cancellation tests share one static appointment object with parallel execution enabled | Whether the production service also has a genuine repeat-cancellation bug that the shared-state test pollution is currently masking |
| ANO-04  | TC-011 | Application defect           | An authenticated user cancelling another user's appointment received 204 instead of 403, reproduced with two different user pairs | Whether the missing authorisation check is isolated to this one endpoint or affects other appointment-modifying endpoints too |
| ANO-05  | TC-015, TC-016 | Environment/dependency incident | DNS resolution for sms-sandbox.test fails from both staging and the environment specialist's independent diagnostic container, matching the provider's own outage notice | How long the sandbox will remain down, and whether the SMS adapter code itself behaves correctly once the dependency is reachable again |

## Issues filed

| Anomaly | Test  | Classification              | GitHub Issue |
|---------|-------|------------------------------|--------------|
| ANO-04  | TC-011 | Application defect           | https://github.com/dhonburi/Week6AppointmentBooking/issues/1 |
| ANO-03  | TC-010 | Test asset or fixture problem | https://github.com/dhonburi/Week6AppointmentBooking/issues/2 |