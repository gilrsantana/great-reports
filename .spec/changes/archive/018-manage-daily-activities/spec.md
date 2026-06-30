# 018-manage-daily-activities

## Objective

Implement the Daily Activity management system. This enables partners to log their daily activities (Title, Theme, Content, Status [Doing/Done], and Blocker indicator) and view/filter their history, respecting the 11:50 PM group timezone lockout rules.

## Technical Context

Since Daily Activity endpoints are not yet implemented in Application or Presentation layers, this specification includes both backend API implementation and frontend UI components.
- Align with: [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md) and framework definitions.
- Timezone rules: Enforce daily activity lockouts at 11:50 PM in the group's local timezone.

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, error validations, labels, and success feedback must be in **BR-Portuguese** (RULE-014).
- The activity logging screen must disable input controls and display a warning banner if the current local time in the group's timezone is after 11:50 PM.
- Display activity status using distinct colors: Doing (Em Andamento - Amber/Indigo) and Done (Concluído - Emerald).

---

## Tasks

### Tasks - Backend Layer (`GreatReports.Presentation` & `Application`)

- [x] Create CQRS use cases in `GreatReports.Application`:
  - Commands: `CreateDailyActivityCommand`, `UpdateDailyActivityCommand` (which must check group lockout time).
  - Queries: `GetPartnerActivitiesQuery` (supporting filters by title, theme, content, status, and date).
>  ✅ 2026-06-30 18:24 - Implemented UpdateDailyActivityCommand and UpdateDailyActivityCommandHandler. Modified CreateDailyActivityCommandHandler to verify group lockout time based on partner's group configurations.
- [x] Create `DailyActivitiesController` (`src/backend/GreatReports.Presentation/Controllers/DailyActivitiesController.cs`):
  - Decorate with `[Authorize(Roles = "Partner")]`.
  - Expose endpoints to log, update, and search daily activities.
>  ✅ 2026-06-30 18:24 - Created PUT endpoint and GET by ID endpoints. Injected IGroupRepository and IDailyActivityRepository.
- [x] Regenerate the OpenAPI frontend client (`npm run generate:api`).
>  ✅ 2026-06-30 18:24 - Ran the dotnet backend service and updated openapi.json, then successfully regenerated frontend client using ng-openapi-gen.

### Tasks - Core Services Wrapper

- [x] Create core wrapper service `src/frontend/src/app/core/services/daily-activity.service.ts`:
  - Expose `logActivity(req: CreateDailyActivityRequest): Promise<string>`.
  - Expose `updateActivity(id: string, req: UpdateDailyActivityRequest): Promise<void>`.
  - Expose `getActivities(filters: ActivityFilters): Promise<PagedResponse<DailyActivityDto>>`.
>  ✅ 2026-06-30 18:24 - Implemented DailyActivityService with logActivity, updateActivity, getActivityById, getLockoutStatus, and getActivities.

### Tasks - Daily Activity UI Components

- [x] Create standalone ActivityLogComponent (`src/frontend/src/app/features/partner/activity-log/activity-log.component.ts`):
  - Forms for Title, Theme, Content, Status (`Doing` vs `Done`), and `IsBlocked` checkbox.
  - Apply lockout logic (checks time and disables form fields).
>  ✅ 2026-06-30 18:24 - Created standalone ActivityLogComponent with full validation, lockout warning banner, form control disabling on lockout, and support for editing.
- [x] Create standalone ActivityHistoryComponent (`src/frontend/src/app/features/partner/activity-history/activity-history.component.ts`):
  - Grid list showing published activities.
  - Add search inputs to filter by Title, Theme, Content, and Date.
>  ✅ 2026-06-30 18:24 - Created standalone ActivityHistoryComponent supporting search filters, status color badges, and conditional edit actions.

### Tasks - Routing Setup

- [x] Wire lazy routing in `src/app/app.routes.ts`:
  - `/parceiro/atividades` -> ActivityHistoryComponent
  - `/parceiro/atividades/registrar` -> ActivityLogComponent
>  ✅ 2026-06-30 18:24 - Configured lazy routes for list, create, and edit activity views.

### Tasks - Verification & Testing

- [x] Write integration tests for backend `DailyActivitiesController` verifying lockout rules at 11:50 PM.
- [x] Write Vitest unit tests for frontend `DailyActivityService` and activity components.
- [x] Run `npm run test` to verify all tests compile and pass.
>  ✅ 2026-06-30 18:24 - Wrote comprehensive backend unit tests for lockout rules and CRUD logic (160 passed), added Vitest tests (14 passed), and confirmed both frontend build and backend tests pass.

---

## Expected Outcome

1. Robust backend endpoints with 11:50 PM timezone lockout validations.
2. Premium partner activity dashboard to log work items.
3. Lockout banner and form disabling when past the daily publishing deadline.
4. Clean and covered unit tests.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
