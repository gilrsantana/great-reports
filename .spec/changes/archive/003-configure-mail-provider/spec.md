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

- [Product Definition](../../../memory/product.md)
- [Global Technical Context](../../../memory/technical-context.md)
- [Repository Structure](../../../memory/structure.md)

## Shared References

- [How to Run](../../../shared/how-to-run.md)
- [Naming Conventions](../../../shared/naming-conventions.md)

---

## Tasks

### Tasks - Domain Layer (`src/backend/GreatReports.Domain`)
- [x] Create domain entity `EmailAuditLog` inheriting from `BaseEntity`:
  - Properties: `Recipient` (string), `Subject` (string), `Body` (string), `SentAt` (DateTimeOffset), `Success` (bool), `ErrorMessage` (string).
>  ✅ 2026-06-28 18:13 - Created rich domain entity EmailAuditLog inheriting from BaseEntity.
- [x] Implement static factory method `Create()` with input validation for email format and non-empty subject.
>  ✅ 2026-06-28 18:13 - Implemented Create() with validation rules and error descriptions in BR-Portuguese.

### Tasks - Application Layer (`src/backend/GreatReports.Application`)
- [x] Define repository interface `IEmailAuditLogRepository` under `Common/Interfaces/`.
>  ✅ 2026-06-28 18:13 - Created IEmailAuditLogRepository interface in Application layer.
- [x] Define service interface `IEmailSender` under `Common/Interfaces/` with method:
  - `Task<Result> SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)`.
>  ✅ 2026-06-28 18:13 - Created IEmailSender interface in Application layer.

### Tasks - Infrastructure Layer (`src/backend/GreatReports.Infrastructure`) (Applying Skill 13)
- [x] Implement `EmailAuditLogConfiguration.cs` inside `Persistence/Configurations/` pluralizing table as `EmailAuditLogs`.
>  ✅ 2026-06-28 18:13 - Configured EmailAuditLogs table mapping with restrictions and length bounds.
- [x] Implement `EmailAuditLogRepository.cs` inheriting from `BaseEntityRepository<EmailAuditLog>`.
>  ✅ 2026-06-28 18:13 - Implemented EmailAuditLogRepository inheriting from BaseEntityRepository.
- [x] Create `MailProviderSettings.cs` options class inside `Configurations/` folder (following Skill 13).
>  ✅ 2026-06-28 18:13 - Created MailProviderSettings options class.
- [x] Create `IMailProviderHttpClientFactory.cs` and its implementation `MailProviderHttpClientFactory.cs` client factory under `Persistence/Mailer/` or `Common/Mailer/` (following Skill 13).
>  ✅ 2026-06-28 18:14 - Created IMailProviderHttpClientFactory and MailProviderHttpClientFactory in Mailer folder.
- [x] Implement typed HTTP clients `MailProviderManagerClient.cs` and `MailProviderSenderClient.cs` (following Skill 13).
>  ✅ 2026-06-28 18:14 - Implemented MailProviderManagerClient and MailProviderSenderClient.
- [x] Implement `MailProviderEmailSender.cs` inheriting from `IEmailSender` (following Skill 13) to format and POST payloads, and log outcomes using `IEmailAuditLogRepository`.
>  ✅ 2026-06-28 18:14 - Implemented MailProviderEmailSender validating invariants, executing POST request and logging outcome.
- [x] Register `MailProviderSettings`, `IMailProviderHttpClientFactory`, `MailProviderManagerClient`, `MailProviderSenderClient`, `IEmailSender`, and `IEmailAuditLogRepository` in `DependencyInjection.cs`.
>  ✅ 2026-06-28 18:14 - Registered all dependency mappings and typed Http clients with Network Credentials in DI extensions.
- [x] Generate and apply EF Core database migrations for `EmailAuditLogs` table.
>  ✅ 2026-06-28 18:16 - Generated AddEmailAuditLogs migration and updated the MySQL database successfully.

### Tasks - Verification & Testing
- [x] Add local dummy/test API credentials to user secrets and verify successful compilation and DI container resolution.
>  ✅ 2026-06-28 18:16 - Added missing ManagerApiKey to user secrets and resolved MySql connection string SslMode compatibility issue.
- [x] Write unit tests verifying:
  - `EmailAuditLog` entity invariants (email formatting, validation error messages in BR-Portuguese following `RULE-014`).
  - `MailProviderEmailSender` logic using mock HTTP client factory and mock repository.
>  ✅ 2026-06-28 18:17 - Wrote comprehensive unit tests, added Infrastructure reference to tests project, and ran all tests successfully.

---

## Expected Outcome

- A fully configured and registered email provider setup with typed clients and factory.
- Database auditing in place for all email transmissions via the `EmailAuditLogs` table.
- Solutions compiling warning-free with zero runtime DI dependency issues.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../../shared/how-to-run.md).
