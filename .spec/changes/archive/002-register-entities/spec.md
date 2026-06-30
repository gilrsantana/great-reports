# 002-register-entities

## Objective

Implement the core entity registration models, use cases, API controllers, and admin dashboard forms. This feature slice enables **Managers** to register Provider Companies, Client Companies, Client Contacts, Projects, and Users (Partners and Group Leaders), and implements the mandatory **Email Verification** and atomic **User-Account Creation (with Rollback)** workflow.

## Technical Context

This feature implements the registration hierarchy of the Great Reports platform. It maps the relations between organizations, defines user roles, splits authentication credentials (`Account`) from domain profile data (`User`), and establishes the email confirmation token workflow required before any dashboard actions are permitted.

> [!IMPORTANT]
> - Relational configurations must enforce `DeleteBehavior.Restrict` on domain boundaries (e.g., deleting a Project must not cascade-delete its Client Company).
> - **Atomic Alignment**: Handlers registering users must create and persist the `User` domain model first. Then, they call `UserManager<Account>.CreateAsync`. If the account creation fails, the handler MUST immediately delete/revert the created `User` record to maintain database consistency.
> - **Schedules**: Scheduled reports configurations must support `Daily`, `Weekly`, `TenDays`, `TwelveDays`, `FifteenDays`, `Monthly`, and `SpecificDay` (supporting specific Day X of the month with query filters targeting from Day X of the previous month to Day X of the current month). Gathers previous task intervals up to the trigger day.
> - Database provider: `MySql.EntityFrameworkCore`.

## Project References

- [Product Definition](../../../memory/product.md)
- [Global Technical Context](../../../memory/technical-context.md)
- [Repository Structure](../../../memory/structure.md)

## Shared References

- [How to Run](../../../shared/how-to-run.md)
- [Naming Conventions](../../../shared/naming-conventions.md)

---

## Tasks

### Tasks - Domain Modeling (`src/backend/great-reports.Domain`)
- [ ] Create domain entities:
  - `ProviderCompany`
  - `ClientCompany`
  - `ClientContact` (properties: Name, Email, ContactType: Commercial/Tech, EmailConfirmed, VerificationToken)
  - `Project`
  - `User` (properties: DisplayName, Email, EmailConfirmed, VerificationToken)
  - `Group` (properties: Name, Timezone, associated entities)
  - `DailyActivity` (properties: Title, Theme, Content, ReferenceDate, Status: ActivityStatus, IsBlocked: bool, IsPublished: bool)
- [ ] Define the `ReportFrequency` enum containing: `Daily`, `Weekly`, `TenDays`, `TwelveDays`, `FifteenDays`, `Monthly`, `SpecificDay`.
- [ ] Define the `ActivityStatus` enum containing: `Doing`, `Done`.
- [ ] Add `SpecificDayOfMonth` integer property to the `ScheduledEmail` entity configuration.
- [ ] Implement static factory methods (`Create()`) performing input validations (e.g. valid email syntax, non-empty names) returning descriptive error messages in **BR-Portuguese** (following `RULE-014`).
- [ ] Implement token generation methods for verification (`GenerateVerificationToken()`) and verification validation (`ConfirmEmail()`).

### Tasks - Application Use Cases (`src/backend/great-reports.Application`)
- [ ] Define repository interfaces in `Common/Interfaces/`:
  - `IProviderCompanyRepository`
  - `IClientCompanyRepository`
  - `IProjectRepository`
  - `IUserRepository`
- [ ] Create Commands:
  - `RegisterProviderCompanyCommand(string Name, string TaxId) : ICommand<Guid>`
  - `RegisterClientCompanyCommand(Guid ProviderCompanyId, string Name) : ICommand<Guid>`
  - `AddClientContactCommand(Guid ClientCompanyId, string Name, string Email, string ContactType) : ICommand<Guid>`
  - `RegisterProjectCommand(Guid ClientCompanyId, string Name, string Description) : ICommand<Guid>`
  - `RegisterUserCommand(Guid ProviderCompanyId, string DisplayName, string Email, string Role) : ICommand<Guid>`
  - `ConfirmEmailCommand(string Email, string Token) : ICommand`
  - `CreateGroupCommand(Guid GroupLeaderId, Guid ClientCompanyId, Guid ProjectId, string Name, string Timezone) : ICommand<Guid>`
- [ ] Create Queries:
  - `GetProviderDetailsQuery(Guid ProviderId) : IQuery<ProviderDetailsDto>`
  - `GetClientCompaniesQuery(Guid ProviderId, int Page, int PageSize) : IQuery<PagedResponse<ClientCompanyDto>>`
- [ ] Implement `RegisterUserCommandHandler` mapping the atomic registration workflow:
  1. Instantiate and save `User` to `IUserRepository` (Domain profile).
  2. Instantiate `Account` (Identity credentials) and call `UserManager<Account>.CreateAsync`.
  3. If `CreateAsync` succeeds, call `UserManager<Account>.AddToRoleAsync(account, command.Role)` to associate standard Microsoft Identity Roles (`GroupLeader` or `Partner`).
  4. If either call fails, call `IUserRepository.Delete(user)` (or remove from DbContext) and commit deletion immediately to rollback and prevent orphaned database profiles.
  5. If successful, generate verification token and dispatch the welcome email.
- [ ] Implement `ConfirmEmailCommandHandler` searching the target user profile and identity account, comparing the token, and updating `EmailConfirmed = true` on both entities.

### Tasks - Infrastructure Mappings & Mailer (`src/backend/great-reports.Infrastructure`)
- [ ] Create Fluent API configuration classes (e.g., `ProviderCompanyConfiguration.cs`, `UserConfiguration.cs`) in `Persistence/Configurations/`:
  - Pluralize tables (`ProviderCompanies`, `ClientCompanies`, `ClientContacts`, `Projects`, `Users`).
  - Add unique index constraints on `TaxId` (Provider) and `Email` (Users, Contacts).
  - Configure `User` relation to `Account` linked via email.
  - Apply `DeleteBehavior.Restrict` on all foreign key mappings.
- [ ] Implement repository classes inheriting from `BaseEntityRepository<T>`.
- [ ] Create `EmailVerificationService` wrapping the **Resend Email API** to dispatch verification token links.
- [ ] Generate and apply database migrations for the new tables.

### Tasks - Presentation Layer (`src/backend/great-reports.Presentation`)
- [ ] Create API Controllers:
  - `ProviderCompaniesController`
  - `ClientCompaniesController`
  - `ProjectsController`
  - `UsersController`
  - `AuthController` (exposing email confirmation endpoint: `POST /api/auth/confirm-email`)
- [ ] Enforce Role Authorization (`[Authorize(Roles = "Manager")]`) on registration endpoints.
- [ ] Register new repositories, CQRS handlers, and the mailer client in `DependencyInjection.cs`.

### Tasks - Frontend Components (`Angular 22`)
- [ ] Define type interfaces in `src/frontend/src/app/core/models/` for all registered entities and request models.
- [ ] Implement HTTP client services (e.g., `company.service.ts`, `user.service.ts`) using standalone injection.
- [ ] Create admin dashboard views under `src/frontend/src/app/features/admin/`:
  - `admin-dashboard.component.ts` (Overview lists)
  - `entity-form.component.ts` (General modal form for registering companies, projects, and users)
- [ ] Create standalone component `email-confirmation.component.ts` in `src/frontend/src/app/features/auth/` containing token submit fields and validation feedback.
- [ ] Ensure all labels, texts, placeholders, validation alerts, and layouts in HTML/TS components use **BR-Portuguese** (following `RULE-014`).
- [ ] Style all pages using Tailwind CSS v4 colors and transitions, using computed Signals to track validation states.

### Tasks - Verification & Testing
- [ ] Write unit tests verifying:
  - Invariant rules of domain entities (validating Name, Email format).
  - Use case flows (Command handlers returning success/failure Results).
  - **Registration Rollback**: Assert that if `UserManager.CreateAsync` fails, the `User` domain entity is successfully removed from the repository.
  - Email confirmation validation logic.
- [ ] Write integration tests verifying database writes and unique email constraints on MySQL.
