# 005-integrate-identity-email-dispatch

## Objective

Integrate Hangfire background processing, the Resend mail provider integration, and ASP.NET Core Identity `IEmailSender<Account>` to enable asynchronous, audit-logged email communications in Portuguese-BR.

## Technical Context

This change ties together the background processing engine (Hangfire) with the email dispatch infrastructure (Resend). Registration confirmation links, password reset links, and verification alerts will be sent asynchronously in the background. The actual email dispatch is handled by `IEmailSender` (powered by Resend HTTP clients) which records each action in `EmailAuditLogs`. Identity features will leverage the generic `IEmailSender<Account>` abstraction.

> [!IMPORTANT]
> - Refer to Skill `14` (Hangfire Background Jobs) and RULE-012 (Background Processing and Orchestration) for background jobs and namespace conventions.
> - Follow localization rules (RULE-014) ensuring all emails dispatched use **BR-Portuguese**.
> - Enqueue job execution against interfaces instead of concrete classes when executing application code (Rule-012 best practice).

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All outgoing email actions (welcome emails, resets, confirmations) should be enqueued via `IBackgroundJobService` to prevent blocking the HTTP request pipeline.
- E-mails must be formatted with readable HTML markup and friendly text in Portuguese-BR.

---

## Tasks

### Tasks - Application Layer (`GreatReports.Application`)
- [x] Create background job class `SendEmailJob` in `src/backend/GreatReports.Application/ApplicationJobs/SendEmailJob.cs` under the namespace `GreatReports.Application.ApplicationJobs` (complying with RULE-012):
  - Injects `IEmailSender` (the application-level dispatcher interface).
  - Implements `Task ExecuteAsync(string recipient, string subject, string body, CancellationToken cancellationToken)`.
>  ✅ 2026-06-28 19:21 - Created SendEmailJob wrapping the dispatcher interface.

### Tasks - Infrastructure Layer (`GreatReports.Infrastructure`)
- [x] Implement `IdentityEmailSender` implementing `IEmailSender<Account>` in `src/backend/GreatReports.Infrastructure/Mailer/IdentityEmailSender.cs` under the namespace `GreatReports.Infrastructure.Mailer`:
  - Injects `IBackgroundJobService`.
  - Enqueues `SendEmailJob` (e.g., calling `Enqueue<SendEmailJob>(job => job.ExecuteAsync(email, subject, body, CancellationToken.None))`) for confirmation links, password reset links, and password reset codes.
>  ✅ 2026-06-28 19:22 - Created IdentityEmailSender formatting rich BR-Portuguese HTML messages and enqueuing background jobs.
- [x] Refactor `EmailVerificationService` in `src/backend/GreatReports.Infrastructure/Mailer/EmailVerificationService.cs`:
  - Injects `IBackgroundJobService`.
  - Instead of mock-logging the mail directly, formats a rich welcome HTML body in BR-Portuguese and enqueues `SendEmailJob`.
>  ✅ 2026-06-28 19:23 - Refactored EmailVerificationService to enqueue verification mail instead of console logs.
- [x] Register `IEmailSender<Account>` as a scoped/transient dependency in `src/backend/GreatReports.Infrastructure/Extensions/DependencyInjection.cs`.
>  ✅ 2026-06-28 19:23 - Registered IEmailSender<Account> as scoped.

### Tasks - Presentation Layer (`GreatReports.Presentation`)
- [x] Register `SendEmailJob` as transient/scoped in `src/backend/GreatReports.Presentation/Extensions/DependencyInjection.cs` composition root (as required by RULE-012 for Hangfire job resolving).
>  ✅ 2026-06-28 19:23 - Registered SendEmailJob as transient in DI.

### Tasks - Verification & Testing
- [x] Implement unit tests for `IdentityEmailSender` in `tests/GreatReports.UnitTests/Infrastructure/IdentityEmailSenderTests.cs` to verify it enqueues verification/reset jobs correctly.
>  ✅ 2026-06-28 19:24 - Implemented unit tests for IdentityEmailSender with dynamic expression argument evaluation.
- [x] Implement unit tests for the updated `EmailVerificationService` in `tests/GreatReports.UnitTests/Infrastructure/EmailVerificationServiceTests.cs` to verify it enqueues the welcome job correctly.
>  ✅ 2026-06-28 19:24 - Implemented unit tests for EmailVerificationService.
- [x] Verify all 136 unit tests build and pass successfully.
>  ✅ 2026-06-28 19:25 - Ran and verified all 138 unit tests pass successfully.

---

## Expected Outcome

- Transparent integration between Identity flows and Hangfire, ensuring registrations, password resets, and verification messages are handled asynchronously in the background.
- Clean execution of emails through Resend with automatic logging in the `EmailAuditLogs` database table.
- Warning-free solution compilation and green unit tests.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
