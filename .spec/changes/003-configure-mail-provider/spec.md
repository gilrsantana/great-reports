# 003-configure-mail-provider

## Objective

Implement the mail provider integration, including option configurations, typed HTTP clients (manager/sender), client factory, email sending service, and email audit logging to enable reliable and trace-monitored email communications.

## Technical Context

This change sets up the core email dispatch infrastructure using ASP.NET Core dependency injection and typed HTTP clients. It implements a client factory to toggle between administrative management and general sending credentials based on the mail provider's configuration. It also records all outgoing emails in the database as audit logs.

> [!IMPORTANT]
> - Refer to Skill `13` (Configure Mail Provider - guidelines for configuring, managing, and verifying the email provider integration) for implementation details.
> - Configure separate HTTP handlers and basic credentials using the `MailProviderSettings` options mapping.
> - Zero cascade deletes on domain boundaries: deleting or auditing emails must not trigger cascading database operations.

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

---

## Tasks

### Tasks - Domain Layer (`src/backend/GreatReports.Domain`)
- [ ] Create domain entity `EmailAuditLog` inheriting from `BaseEntity`:
  - Properties: `Recipient` (string), `Subject` (string), `Body` (string), `SentAt` (DateTimeOffset), `Success` (bool), `ErrorMessage` (string).
- [ ] Implement static factory method `Create()` with input validation for email format and non-empty subject.

### Tasks - Application Layer (`src/backend/GreatReports.Application`)
- [ ] Define repository interface `IEmailAuditLogRepository` under `Common/Interfaces/`.
- [ ] Define service interface `IEmailSender` under `Common/Interfaces/` with method:
  - `Task<Result> SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)`.

### Tasks - Infrastructure Layer (`src/backend/GreatReports.Infrastructure`) (Applying Skill 13)
- [ ] Implement `EmailAuditLogConfiguration.cs` inside `Persistence/Configurations/` pluralizing table as `EmailAuditLogs`.
- [ ] Implement `EmailAuditLogRepository.cs` inheriting from `BaseEntityRepository<EmailAuditLog>`.
- [ ] Create `MailProviderSettings.cs` options class inside `Configurations/` folder (following Skill 13).
- [ ] Create `IMailProviderHttpClientFactory.cs` and its implementation `MailProviderHttpClientFactory.cs` client factory under `Persistence/Mailer/` or `Common/Mailer/` (following Skill 13).
- [ ] Implement typed HTTP clients `MailProviderManagerClient.cs` and `MailProviderSenderClient.cs` (following Skill 13).
- [ ] Implement `MailProviderEmailSender.cs` inheriting from `IEmailSender` (following Skill 13) to format and POST payloads, and log outcomes using `IEmailAuditLogRepository`.
- [ ] Register `MailProviderSettings`, `IMailProviderHttpClientFactory`, `MailProviderManagerClient`, `MailProviderSenderClient`, `IEmailSender`, and `IEmailAuditLogRepository` in `DependencyInjection.cs`.
- [ ] Generate and apply EF Core database migrations for `EmailAuditLogs` table.

### Tasks - Verification & Testing
- [ ] Add local dummy/test API credentials to user secrets and verify successful compilation and DI container resolution.
- [ ] Write unit tests verifying:
  - `EmailAuditLog` entity invariants (email formatting, validation error messages in BR-Portuguese following `RULE-014`).
  - `MailProviderEmailSender` logic using mock HTTP client factory and mock repository.

---

## Expected Outcome

- A fully configured and registered email provider setup with typed clients and factory.
- Database auditing in place for all email transmissions via the `EmailAuditLogs` table.
- Solutions compiling warning-free with zero runtime DI dependency issues.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
