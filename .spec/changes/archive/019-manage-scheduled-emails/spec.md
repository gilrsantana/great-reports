# 019-manage-scheduled-emails

## Objective

Implement the Scheduled Email configuration UI for Group Leaders. This allows them to create and update scheduling profiles (Daily, Weekly, Monthly, or Specific Days) and assign target receivers (Managers, Partners, Client Contacts) for automated report summaries.

## Technical Context

Since Scheduled Email and Receiver endpoints are not yet fully mapped in Application/Presentation layers, this specification covers both backend exposing and frontend UI component tasks.
- Align with: [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md) and framework definitions.
- Frequency Options: Daily (Diário), Weekly (Semanal), TenDays (10 Dias), TwelveDays (12 Dias), FifteenDays (15 Dias), Monthly (Mensal), or Specific Day of Month (Dia Específico).

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, error validations, labels, and success feedback must be in **BR-Portuguese** (RULE-014).
- Allow scheduling configuration directly inside the Group details view as a sub-panel or tab.
- Input validation: If frequency is "Specific Day of Month", the day field must be between 1 and 31.

---

## Tasks

### Tasks - Backend Layer (`GreatReports.Presentation` & `Application`)

- [x] Create CQRS commands and queries:
  - Commands: `CreateScheduledEmailCommand`, `AddScheduledEmailReceiverCommand`.
  - Queries: `GetGroupScheduledEmailsQuery`.
>  ✅ 2026-06-30 18:32 - Implemented CreateScheduledEmailCommand, AddScheduledEmailReceiverCommand, and GetGroupScheduledEmailsQuery.
- [x] Create `ScheduledEmailsController` (`src/backend/GreatReports.Presentation/Controllers/ScheduledEmailsController.cs`):
  - Decorate with `[Authorize(Roles = "GroupLeader,Manager")]`.
  - Expose endpoints to manage scheduled emails and receivers.
>  ✅ 2026-06-30 18:32 - Created controller exposing endpoints to create schedules, add receivers, and fetch group schedules.
- [x] Regenerate the OpenAPI frontend client (`npm run generate:api`).
>  ✅ 2026-06-30 18:32 - Regenerated frontend API models and services using ng-openapi-gen.

### Tasks - Core Services Wrapper

- [x] Create core wrapper service `src/frontend/src/app/core/services/scheduled-email.service.ts`:
  - Expose `createScheduledEmail(req: CreateScheduledEmailRequest): Promise<string>`.
  - Expose `addReceiver(emailId: string, req: AddReceiverRequest): Promise<void>`.
  - Expose `getGroupSchedules(groupId: string): Promise<ScheduledEmailDto[]>`.
>  ✅ 2026-06-30 18:32 - Created ScheduledEmailService wrapping the regenerated OpenAPI client functions.

### Tasks - Scheduled Email UI Components

- [x] Create standalone ScheduledEmailConfigComponent (`src/frontend/src/app/features/group-leader/scheduled-email-config/scheduled-email-config.component.ts`):
  - Form validations for Frequency, CronExpression, and Specific Day of Month.
  - Checklist of potential receivers (Group Leader, Partners, Managers, Client Contacts).
  - Submit action invoking `ScheduledEmailService`.
>  ✅ 2026-06-30 18:32 - Created ScheduledEmailConfigComponent supporting frequency select, specific day validation, and target receiver drop-downs.

### Tasks - Routing Setup

- [x] Integrate configuration panel as a view/tab within the Group details route:
  - `/lider/grupos/:id` -> GroupDetailsComponent (containing ScheduledEmailConfigComponent).
>  ✅ 2026-06-30 18:32 - Created GroupDetailsComponent and wired lazy-loaded route in app.routes.ts.

### Tasks - Verification & Testing

- [x] Write integration and unit tests for backend `ScheduledEmailsController`.
- [x] Write Vitest unit tests for frontend `ScheduledEmailService` and configuration component.
- [x] Run `npm run test` to verify all tests compile and pass.
>  ✅ 2026-06-30 18:32 - Created unit tests for the command handlers in backend, wrote Vitest unit tests for the service in frontend, and verified both backend and frontend test suites pass.

---

## Expected Outcome

1. Backend controller (`ScheduledEmailsController`) exposed in Scalar/OpenAPI.
2. Premium email scheduling configuration panel.
3. Form validation checking frequencies and specific day boundaries in BR-Portuguese.
4. Clean and covered unit tests.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
