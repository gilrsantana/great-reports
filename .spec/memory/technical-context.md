# Global Technical Context

## Base Stack

- **Backend**: .NET 10 Clean Architecture (located under `src/backend/`)
  - Target Framework: `net10.0`
  - Solution Name: `GreatReports`
  - Projects:
    - `GreatReports.Domain`: Rich domain entities (including `User`, `ProviderCompany`, `ClientCompany`, `ClientContact`, `Project`, `Group`, `ScheduledEmail`), value objects, domain validations, base primitives. Zero dependencies on external layers/frameworks (except Shared).
    - `GreatReports.Application`: Use cases, CQRS commands/queries, handlers, repository interfaces, DTOs, Application Jobs, and email processing services.
    - `GreatReports.Infrastructure`: Database context (`GreatReportsDbContext`), EF Core configurations (using `MySql.EntityFrameworkCore` provider), repositories, migrations, `Microsoft.AspNetCore.Identity` services (managing the `Account` and `Role` entities), Resend mail client integration, and Hangfire background configuration.
    - `GreatReports.Presentation`: Web API controllers inheriting from `ApiControllerBase`, composition root, middleware, OpenAPI, Scalar API Reference, and Hangfire Dashboard access control.
    - `GreatReports.Shared`: Result patterns, basic error definitions.
- **Frontend**: Angular 22 Web Application (located under `src/frontend/`)
  - Language: TypeScript
  - Style Framework: Tailwind CSS v4 (using custom theme variables)
  - Core Rules: Standalone components, Reactive Signals for state management, property-level Dependency Injection (`inject()`), RxJS `firstValueFrom` for Promises in async actions, role-based client dashboard rendering.
- **Database**: MySQL.
  - ORM: Entity Framework Core (configured via `MySql.EntityFrameworkCore`).
- **Background Engine**: Hangfire with MySQL storage backend, handling scheduled job triggers.
- **External APIs**:
  - **Gemini LLM API**: Summarizes group daily activity compilations.
  - **Resend Email API**: Sends generated email reports and verification alerts.
- **API Protocol**: REST over JSON, with error responses utilizing the RFC 7807 `ProblemDetails` standard.

## Security & Registration Workflows

- **Separation of User and Account**:
  - **`Account`**: Managed by `Microsoft.AspNetCore.Identity`. Represents credentials, password hashes, lockouts, and roles. Extends `IdentityUser<Guid>` and lives inside `great-reports.Infrastructure`.
  - **`User`**: A domain entity representing personal information, display names, and profile state. Extends `BaseEntity` and lives inside `great-reports.Domain`.
  - **Linking**: Connected via a unique `Email` field.
  - **Atomic Creation & Rollback**: When registering a new person, the application must perform creation atomically:
    1. The `User` domain entity is created and saved to the database.
    2. The `Account` identity credentials are created via the `UserManager<Account>` wrapper service.
    3. If the `Account` creation fails, the application must immediately delete/revert the created `User` entity to prevent orphaned domain entries.
- **Email Verification**: Every person added to the system must validate their email through a secure verification token sent to their email before they can access dashboards or participate in regular system processes.
- **Role-Based Access Control (RBAC)**: All authorization roles are managed strictly via `Microsoft.AspNetCore.Identity` Roles (bound to the `Account` entity in the database). Endpoints and dashboards verify these roles via authenticated JWT Claims:
  - **Manager**: Register provider companies, clients, projects, partners, and group leaders.
  - **Maintainer**: Has all Manager permissions plus full access to the Hangfire Dashboard.
  - **GroupLeader**: Manage groups, associate partners, client companies, projects, client contacts, and scheduled emails.
  - **Partner**: Log daily activities (done/doing). Can belong to multiple groups.
  - **Client**: View read-only dashboards for their own company and receive reports according to scheduling profiles.

## Background Processing & LLM Rules

- **Timezone Schedules**: Recurring jobs must process lockouts (11:50 PM) and deliveries (8:00 AM) based on the group's local timezone. The scheduling module translates cron expressions and offsets based on the target group's timezone identifier.
- **Group-Level Aggregation**: Report compile jobs fetch all published activities from all partners associated with the target group during the reporting timeframe, concatenating them into a single payload for the Gemini API.
- **Gemini Fail-Safe Alert**: If the Gemini API fails, the system executes 3 retries with exponential backoff. If processing fails persistently, the report compilation is suspended, and the Resend client immediately dispatches a detailed diagnostic alert email to the Group Leader, Managers, and Maintainers.

## Architectural Decisions

- **CQRS Hand-Rolled**: Direct Dependency Injection of command/query handlers instead of MediatR/mediator frameworks.
- **Result Pattern**: Business logic failures return explicit `Result` or `Result<T>` types instead of throwing exceptions.
- **Base Entity**: All domain entities inherit from `BaseEntity` (utilizing UUID v7 and timestamp tracking).
- **Repositories & UoW**: Repositories inherit from `BaseEntityRepository<TEntity>` which implements `IUnitOfWork`. Logical inactivation is used instead of physical removal.
- **API Documentation**: Scalar API Reference is used in the development environment, replacing Swagger.
- **Styling**: Strict adherence to Tailwind CSS v4 theme tokens without hardcoded styling.
- **Localization & Language Boundary (RULE-014)**: Code structures, database schemas, and keys use **US-English**. Error messages (descriptions inside `Result` outcomes) and the Frontend UI (components, templates, layouts, charts) use **BR-Portuguese**. All emails dispatched by Resend use **BR-Portuguese**.

## Constraints

- Changes must be incrementally specified and implemented (one spec per feature/delivery).
- Maintain clean, minimal abstractions suitable for educational and reference purposes.
- Zero cascade deletes on domain boundaries (use `DeleteBehavior.Restrict`).
