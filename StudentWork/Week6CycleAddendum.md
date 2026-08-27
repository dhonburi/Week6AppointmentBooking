# Week 6 Test-Cycle Addendum

## Baseline

- Build/commit: pending first commit on week6-test-management
- OS: Windows 11 Home, version 25H2, OS build 26200.9168
- .NET SDK version: 10.0.400
- Test command: Visual Studio Test Explorer. Run All Tests (equivalent to dotnet test Starter/AppointmentBooking.slnx)
- Execution date: 26/08/2026
- Tests discovered: 12
- Tests passed: 12
- Tests failed: 0

**Checkpoint question:** If every supplied domain-level test passes, what important
confidence is still missing before an integrated release decision can be made?

1. The domain-level suite only exercises the Doctor/AppointmentRequest classes in
   isolation. It says nothing about whether the service layer, staging database,
   or SMS integration behave correctly once wired together.
2. It also can't catch concurrency or authorisation problems that only show up
   under real request timing or a real auth layer. e.g. two simultaneous
   bookings for the last slot (TC-008) or one user cancelling another user's
   appointment (TC-011). A unit test run sequentially will never surface these.

## Release and objective

Release/build: AB-1.4-RC1
Decision this cycle must support: Whether to fully release, restrict, or delay AB-1.4-RC1
for the supervised pilot

## Readiness decision

Decision: start partially

Rationale: Three of four entry criteria are fully met, and no open Critical or Severity
defect is known before execution begins. The only gap is the SMS sandbox outage, which
affects a narrow, identifiable slice of the portfolio (TC-015 and TC-016, both tied to
REQ-SMS-01/02) rather than the whole release scope. The remaining 18 test cases, covering
booking, validation, cancellation, authorisation, persistence and audit, have no dependency
on the SMS sandbox and can proceed immediately with the staging environment and prepared
test data already available. Suspending the entire cycle over one unavailable external
dependency would waste the limited tester time available (7 person hours across Tester A
and B) on scope that isn't actually affected by the outage.

Permitted scope: TC-001–TC-014, TC-017–TC-020 (18 of 20 test cases)
Blocked scope: TC-015, TC-016 (SMS reminder functionality). Pending sandbox recovery,
to be picked up by the environment specialist from 1:00 pm


| Entry criterion | Met / Partly / Not met | Evidence | Consequence |
|---|---|---|---|
| RC deployed to a stable staging environment | Met | Release brief confirms AB-1.4-RC1 is deployed to staging with app and database available at cycle start | Testing can begin immediately |
| Domain level automated baseline passes | Met | Activity 1 baseline: 12/12 MSTest domain tests passed, 0 failed | Confidence in core domain logic before integration level testing |
| Test users, doctors, appointments and DB connectivity prepared | Met | Release brief: accounts prepared. Test database responds to the environment smoke check | Data dependent cases can execute without setup delay |
| All planned test dependencies available for the full portfolio | Not met | SMS sandbox does not currently resolve from staging. Recovery time unconfirmed | SMS dependent cases (TC-015, TC-016) cannot execute until recovery is confirmed. Restricts scope rather than blocking the whole cycle |


## Exit criteria

1. All Critical risk test cases (TC-008, TC-011, TC-014) have been executed, with no
   unresolved Critical or Severity 1 defect remaining open.
2. State integrity and authorisation scenarios (TC-006, TC-009, TC-010, TC-011) pass,
   giving confidence that slot counts stay accurate and users can't access or cancel
   appointments they don't own.
3. Any test case that remains untested, blocked or failed at the end of the cycle is
   explicitly documented with its residual risk, rather than silently left out of the
   release decision.
4. Every defect fixed during the cycle has both confirmation evidence (the original
   failure no longer reproduces) and targeted regression evidence (the fix hasn't
   broken related behaviour) before it's counted as resolved.

## Suspension and resumption

Suspension condition: If a Critical risk defect is found that suggests broader state
corruption or a security bypass beyond its own test case (for example, if the
authorisation bypass surfaced by TC-011 is found to affect other endpoints, or a
booking integrity issue is found to corrupt data beyond the specific scenario tested),
further execution in the affected area should be suspended until the scope of the
problem is understood.

Resumption evidence required: A documented explanation of the defect's actual scope
(confirming it's contained to the originally identified scenario, or that a fix has
been applied), plus a passing confirmation test for the original failure, before
resuming broader execution in that area.

## Work-breakdown estimate

| Work item                                   | Effort | Dependency                          | Parallel? | Assumption                                                                                     |
|----------------------------------------------|-------:|--------------------------------------|-----------|--------------------------------------------------------------------------------------------------|
| Environment smoke checks                      | 0.50h  | Staging build and accounts ready     | Partly    | App and database checks can be split between Tester A and B, so wall time is under 30 minutes    |
| Prepare and verify test-data sets             | 1.00h  | Environment smoke checks complete    | Yes       | Both testers can prepare sets at the same time. Synthetic data only                              |
| Execute planned test cases                    | 4.00h  | Environment and data ready           | Yes       | 20 cases split across Tester A and B. TC-015 and TC-016 depend on the SMS sandbox and may need to be deferred until it recovers |
| Investigate and triage anomalies              | 1.67h  | Initial results and evidence exist   | Partly    | Testers do initial classification alone. The developer only joins for the harder cases, once classification narrows scope |
| Confirmation and targeted regression tests    | 1.50h  | A resolved build from the developer  | Yes       | Assumes 6 tests is the right scope. May grow if triage in Activity 3 finds more than expected    |
| Prepare progress report                       | 0.50h  | Checkpoint A evidence available      | No        | Single-author task, but can be written while anomaly investigation is still ongoing              |
| Prepare completion report                     | 0.75h  | Final evidence and release decision  | No        | Cannot start until confirmation, regression, and residual risks are settled. Genuinely last      |

Total estimated person-hours: 9.92 (9 hours 55 minutes)

Estimated calendar duration: approx. 7 hours, assuming smoke checks, data prep, and
execution run in parallel across Tester A and B, triage overlaps with progress-report
writing, and confirmation/regression plus the completion report run sequentially at
the end once a resolved build and final evidence exist.

Main uncertainty: the total estimated effort (9.92 person-hours) is very close to the
combined capacity of Tester A, Tester B and the developer (4 + 3 + 2 = 9 hours), leaving
almost no slack. If the SMS sandbox doesn't recover in time to execute TC-015/016 or the 
confirmation/regression scope grows past 6 tests, the cycle will not fit inside the 
available hours before the deadline.