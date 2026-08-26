# Week 6 Test Progress Report

## Reporting point

Release/build: AB-1.4-RC1
Evidence source: LabData/CheckpointA.csv, LabData/TestPortfolio.csv
Definitions used: Executed means a Passed or Failed result only. Blocked and Not Run are
tracked as separate, distinct statuses, not counted as executed. Planned is the full
20-test portfolio in TestPortfolio.csv.

## Metrics

| Metric                 | Formula and values         | Result | Interpretation |
|-------------------------|------------------------------|-------:|-----------------|
| Execution progress      | 15 executed ÷ 20 planned     | 75.0%  | Shows three-quarters of the planned portfolio has been attempted. It does not show whether that work succeeded, and it treats the 2 blocked tests the same as the 3 not-yet-started tests, even though those are different problems: one is an external dependency outage, the other is simply unscheduled. |
| Pass rate               | 11 passed ÷ 15 executed       | 73.3%  | Shows how much of the attempted work met its expected result. It says nothing about the 5 tests with no result yet, several of which sit in the highest-risk category, so it can't be read as "73% of the release is confirmed correct." |
| Blocked proportion      | 2 blocked ÷ 20 planned        | 10.0%  | Shows how much of the whole portfolio currently can't produce trustworthy evidence because of the SMS sandbox outage. On its own it looks small, but one of the two blocked tests (TC-016) is High risk, so the percentage alone hides that. |
| High/Critical coverage  | 9 executed High/Critical ÷ 13 planned High/Critical | 69.2% | Shows how much of the highest-consequence scope has actually been exercised, the most decision-relevant number here. But "executed" does not mean "passed": several of these 9 executed tests actually failed, so this number needs to be read alongside the pass/fail split, not alone. |

**Why a tool counting Blocked as Executed would differ:** treating TC-015 and TC-016 as
executed would push execution progress to (15+2)/20 = 85% instead of the real 75%.
That overstates readiness in a way that matters here specifically, because TC-016 is a
High-risk SMS resilience test that has produced zero actual evidence. An 85% figure
would make the release look nearly test-complete when a real gap still exists.

**A risk the overall pass rate hides:** all three Critical-risk tests in this portfolio
(TC-008, TC-011, TC-014) currently show either Failed or Not Run. None has passed. The
73.3% aggregate pass rate blends this in with lower-risk passes and reads as reasonably
healthy, when in fact zero Critical-risk evidence currently supports a release decision.

## Status and forecast

Important evidence: 11 passed, 4 failed (clustered around booking-retry, concurrency and
cancellation/authorisation behaviour), 2 blocked (SMS sandbox, external), 3 not run
(including TC-014, a Critical test with no evidence yet).

Main blockers: the SMS sandbox outage has no confirmed recovery time. The four failures
cluster tightly enough around retry/concurrency/state-handling that they may share a root
cause, which needs developer diagnosis before confirmation work can start. TC-013, TC-014
and TC-018 haven't been scheduled yet.

Forecast against the plan: the Activity 2 estimate had almost no slack (9.92 hours of
effort against roughly 9 hours of combined tester and developer capacity). With 4 failures
needing investigation and 3 tests still not run, including a Critical one, the cycle is
behind where a smooth, mostly-passing run would have it at this checkpoint. The original
estimate assumed a straightforward portfolio, not a defect cluster requiring diagnosis.

## Control actions

| Action | Signal that triggered it | Expected benefit | Trade-off or new risk | Owner |
|---|---|---|---|---|
| Early execution of unexecuted Critical tests | All 3 Critical tests (TC-008, TC-011, TC-014) are currently Failed or Not Run. None passes | Closes the biggest blind spot in the highest-risk category before the deadline | Pulls a tester off confirmation/regression retesting of the known failures, pushing that work later and compressing an already-tight estimate | Tester A |
| Targeted investigation and regression around the defect cluster | TC-007, TC-008, TC-010 and TC-011 all fail and cluster around retry/concurrency/state-handling on booking and cancellation | If a shared root cause exists, one fix could resolve several failures instead of four separate patches | Pulls developer hours forward from the budgeted post-triage slot into diagnosis now, leaving less time for build fixes later, and risks time spent chasing a cause that may not be shared | Developer |
| Deferral of lower-risk scope with explicit residual risk | TC-013 and TC-018 are Not Run but not Critical. TC-015/016 remain blocked with unknown recovery time | Frees capacity to prioritise Critical-risk work and the defect cluster within the tight time budget | Creates real residual risk: if the cycle ends before TC-013 runs, the release decision is made with no evidence on whether a booking survives a service restart | Test lead |

## Communication required

The clinic stakeholder should be told now, not at the 4:00 pm deadline, that all three
Critical-risk tests currently show no passing evidence and that the SMS sandbox outage
has no confirmed recovery time. Both directly affect whether a decision can defensibly
be made on schedule.