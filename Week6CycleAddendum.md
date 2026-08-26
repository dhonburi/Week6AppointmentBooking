# Week 6 Test-Cycle Addendum

## Baseline

- Build/commit: pending first commit on week6-test-management
- OS: Windows 11 Home, version 25H2, OS build 26200.9168
- .NET SDK version: 10.0.400
- Test command: Visual Studio Test Explorer — Run All Tests (equivalent to dotnet test Starter/AppointmentBooking.slnx)
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

Release/build:
Decision this cycle must support:

## Readiness decision

Decision: start fully / start partially / suspend

Rationale:

| Entry criterion | Met / Partly / Not met | Evidence | Consequence |
|---|---|---|---|
| | | | |
| | | | |
| | | | |

## Exit criteria

1.
2.
3.
4.

## Suspension and resumption

Suspension condition:
Resumption evidence required:

## Work-breakdown estimate

| Work item | Effort | Dependency | Can run in parallel? | Assumption |
|---|---:|---|---|---|
| | | | | |

Total estimated person-hours:
Estimated calendar duration:
Main uncertainty:
