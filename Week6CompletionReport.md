# Week 6 Test Completion Report

## 1. Release and scope

Release/build: AB-1.4-RC3 (final snapshot for this cycle)
Scope tested: 17 of 20 planned test cases executed (Passed or Failed), covering booking,
validation, state integrity, cancellation, authorisation, persistence, SMS resilience,
end-to-end booking, usability messaging.
Scope not tested or blocked: TC-013 (restart persistence) remained Blocked, restart
window was not made available. TC-018 (end-to-end cancellation) and TC-020 (audit log)
were deferred and not run before the deadline.

## 2. Final results

Definitions and calculations: Executed means a Passed or Failed result only, consistent
with Activity 1 and Activity 4. Planned is the full 20-test portfolio.

- Execution progress: 17 executed ÷ 20 planned = 85.0%
- Pass rate: 15 passed ÷ 17 executed = 88.2%
- Blocked proportion: 1 blocked ÷ 20 planned = 5.0%
- High/Critical coverage: 11 executed High/Critical ÷ 13 planned High/Critical = 84.6%

All four metrics improved from the Activity 4 checkpoint (75.0%, 73.3%, 10.0%, 69.2%
respectively). However, the pass rate improvement does not mean the release is clean:
of the three Critical-risk test cases, TC-011 and TC-014 now pass, but TC-008 still
fails. One of three Critical-risk scenarios remains unresolved at the end of the cycle.

## 3. Deviations from the cycle addendum

The original Activity 2 plan assumed the 18 non-SMS-dependent tests could proceed
immediately and the 2 SMS tests would resume once the sandbox recovered. That held:
the sandbox recovered, TC-015 passed, and TC-016 was executed, but it revealed a new,
genuine defect (DEF-003) rather than confirming the feature was safe.

The regression discovered in Activity 5 (TC-017 failing after the DEF-001 fix) was not
part of the original estimate and required extra investigation and a follow-up fix,
consuming time that was not budgeted in Activity 2.

TC-013 was already flagged as a likely deferral candidate in the Activity 4 control
actions, given the tight estimate. That forecast held: it remained blocked through to
the end. TC-018 and TC-020 were similarly deferred under time pressure, consistent with
the near-zero slack identified in the Activity 2 work-breakdown estimate.

## 4. Exit-criteria assessment

| Exit criterion | Met / Not met | Evidence | Decision consequence |
|---|---|---|---|
| All Critical-risk test cases executed with no unresolved Critical defect open | Not met | TC-008, TC-011, TC-014 are all executed, but TC-008 still fails: an unhandled exception remains possible under concurrent final-slot requests | A Critical-risk concurrency defect remains open at the point of decision. This must be explicitly accepted or mitigated, not silently released |
| State-integrity and authorisation scenarios pass | Met | TC-006, TC-009, TC-010, TC-011 all passed in the final snapshot | Confidence in slot-count accuracy and cancellation ownership is supported by evidence |
| Untested/blocked/failed scope is explicitly documented with residual risk | Met | This report documents TC-008, TC-013, TC-016, TC-018 and TC-020 in Section 5 | The release decision is made with visible gaps rather than hidden ones |
| Defects counted as resolved have both confirmation and regression evidence | Met | DEF-001 has both confirmation (TC-007) and regression (TC-017) evidence passing. DEF-002 and TEST-001 are similarly supported. DEF-003 is correctly still open, not falsely closed | No known defect has been closed on confirmation evidence alone |

## 5. Remaining defects and residual risks

| Defect or gap | Exposure and impact | Mitigation | Risk owner | Follow-up |
|---|---|---|---|---|
| TC-008 unresolved (unhandled exception under concurrent final-slot requests, Critical) | Two patients racing for the last slot can see a raw, unhandled error rather than a clean "slot unavailable" message. The database itself does not corrupt. The actual booking data stays correct | Monitor error logs closely during the supervised pilot. Staff can assist a patient who hits the error | Developer / release manager | Fix scheduled before any release beyond the supervised, staff-present pilot |
| TC-016 / DEF-003 unresolved (SMS provider error still rolls back a valid booking) | A confirmed, valid booking can be silently destroyed by an unrelated SMS provider failure, directly contradicting the release scope's explicit requirement that reminder failure must not undo a valid booking | Disable or defer the automatic SMS reminder trigger during the pilot so the vulnerable code path is never invoked | Developer | Fix and re-verify before SMS reminders are enabled in any release |
| TC-013 blocked (restart persistence not verified) | Unknown whether a booking survives an application or service restart during the pilot window | Avoid planned restarts during the pilot. If a restart is unavoidable, manually verify a sample of bookings survive it | Test lead / operations | Schedule a dedicated restart-window test before pilot scope expands |
| TC-018 and TC-020 not run (end-to-end cancellation, audit log) | No end-to-end evidence for cancellation completeness or audit-log correctness, which matters for accountability in a clinical system, even though component-level cancellation tests did pass | Treat audit-log behaviour as unverified, not assumed correct, until tested | Test lead | Run both before any exit from the supervised-pilot phase |

## 6. Release recommendation

Recommendation: Restricted release

Evidence-based rationale: Fifteen of seventeen executed tests pass, including all state-
integrity and authorisation scenarios, which supports proceeding rather than delaying
outright. However, two things block a full release. First, DEF-003 lets an unrelated
SMS provider failure destroy a valid, confirmed booking, directly contradicting an
explicit release-scope requirement. This can be fully removed as a risk by disabling the
SMS reminder trigger for the pilot. Second, TC-008 is a Critical-risk exit criterion
that remains unmet. The exposure is a raw error rather than data corruption, and the
pilot's staff supervision provides a partial compensating control, but this is a real
residual risk that needs explicit sign-off, not silent acceptance.

Restrictions or conditions, if any:
- SMS reminders disabled or deferred until DEF-003 is fixed and re-verified.
- The concurrent final-slot race (TC-008) is accepted as a residual risk specifically
  for the supervised pilot, with active error-log monitoring. It must be fixed before
  any wider or unsupervised rollout.
- No planned service restarts during the pilot window, given TC-013 is unverified.
- TC-018 and TC-020 must be run and pass before the pilot scope is expanded further.
- The clinic stakeholder must formally accept the TC-008 residual risk before the
  pilot begins. This cannot be accepted implicitly by proceeding with the release.

## 7. Copilot challenge and human judgement

Prompt used: Pasted the Section 6 recommendation (Restricted release, with the SMS
disable and TC-008 accepted-risk conditions) and asked: "Act as a sceptical release
manager. Challenge the recommendation below. Identify missing evidence, unsupported
assumptions and residual risks. Do not replace the final human decision."

Useful challenge accepted: Copilot pointed out there's no proof that disabling the SMS
trigger actually removes the DEF-003 failure path. If the SMS call is synchronous and
inside the same booking transaction, a config flag alone might not stop the faulty code
path, since the side effect could still be baked into the booking flow. I hadn't verified
this. I accepted this and would add a step before the pilot: confirm in staging that
disabling the trigger genuinely prevents the rollback behaviour, not just assume it does
because the reminder no longer sends.

Suggestion rejected or modified: Copilot listed "double-bookings" and "data corruption"
as a residual risk from TC-008. I'm rejecting that specific claim, because it isn't
supported by the actual test evidence: across all 10 concurrent runs in the original
TC-008 evidence, the database never allowed more than one booking to succeed. The only
observed defect is an unhandled exception returned to the losing request. Copilot was
working from the recommendation text and the code alone, without that evidence, so it
reasoned from generic race-condition risk rather than what was actually measured. I'd
modify the risk description to "user-facing errors under a rare race condition" rather
than "data corruption," since overstating the risk beyond what the evidence shows isn't
more careful, it's just inaccurate.

Why human judgement was required: Copilot's full list of conditions (root-cause traces,
a dependency matrix, automated backup snapshots, formal monitoring and alerting) reads
like the bar for a general production release, not a narrow, time-boxed, staff-supervised
pilot using synthetic data. Deciding how much of that rigor is actually proportionate
here, versus what can reasonably be phased in before a wider rollout, depends on
operational context and risk tolerance that the evidence alone doesn't settle. That's a
judgement call for a human release manager, not something derivable purely from a code
review or a generic risk checklist.