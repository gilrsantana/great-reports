# 020-view-email-audit-logs

## Objective

Implement the Email Audit Logs frontend view for Maintainers and Managers. This enables them to monitor email delivery records (Subject, Recipient, Status [Sent/Failed], and error/diagnostic details for failed deliveries).

## Technical Context

Since Email Audit Log query endpoints are not yet implemented in Application/Presentation layers, this specification includes both backend API mapping and frontend UI components.
- Align with: [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md) and framework definitions.
- Security: Access is restricted strictly to users with the `Maintainer` or `Manager` role.

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, status indicators, and feedback messages must be in **BR-Portuguese** (RULE-014).
- Failed logs must show a badge or warning style, and provide a button to open a detailed modal showing the raw error response or stack trace.
- Timestamps must be displayed in `JetBrains Mono` and formatted using the user's browser local format.

---

## Tasks

### Tasks - Backend Layer (`GreatReports.Presentation` & `Application`)

- [x] Create CQRS Query `GetEmailAuditLogsQuery` in `GreatReports.Application` supporting pagination, status filters, and recipient search.
>  ✅ 2026-06-30 18:36 - Created GetEmailAuditLogsQuery, EmailAuditLogDto, and query handler querying database using IEmailAuditLogRepository.
- [x] Create `EmailAuditLogsController` (`src/backend/GreatReports.Presentation/Controllers/EmailAuditLogsController.cs`):
  - Decorate with `[Authorize(Roles = "Maintainer,Manager")]`.
  - Expose `GET /api/EmailAuditLogs` returning `ProducesResponseType(typeof(PagedResponse<EmailAuditLogDto>), StatusCodes.Status200OK)`.
>  ✅ 2026-06-30 18:36 - Implemented EmailAuditLogsController returning paginated responses.
- [x] Regenerate the OpenAPI frontend client (`npm run generate:api`).
>  ✅ 2026-06-30 18:36 - Regenerated API files using ng-openapi-gen.

### Tasks - Core Services Wrapper

- [x] Create core wrapper service `src/frontend/src/app/core/services/email-audit-log.service.ts`:
  - Expose `getEmailLogs(page: number, pageSize: number, filters: LogFilters): Promise<PagedResponse<EmailAuditLogDto>>`.
>  ✅ 2026-06-30 18:36 - Created EmailAuditLogService to fetch logs.

### Tasks - Email Audit Logs UI Components

- [x] Create standalone EmailAuditListComponent (`src/frontend/src/app/features/admin/email-audit-list/email-audit-list.component.ts`):
  - Paginated table showing: Data/Hora, Destinatário, Assunto, Status (badge styling: Success/Failed).
  - Search fields to filter by Destinatário or Status.
>  ✅ 2026-06-30 18:36 - Created EmailAuditListComponent with filters, pagination, and details modal link.
- [x] Create standalone LogDetailsModalComponent (`src/frontend/src/app/features/admin/log-details-modal/log-details-modal.component.ts`):
  - Modal displaying full log details and raw error details (using `JetBrains Mono` formatting).
>  ✅ 2026-06-30 18:36 - Created LogDetailsModalComponent using JetBrains Mono formatting for error stack trace.

### Tasks - Routing Setup

- [x] Wire lazy routing in `src/app/app.routes.ts`:
  - `/admin/logs-email` -> EmailAuditListComponent
>  ✅ 2026-06-30 18:36 - Integrated route `/admin/logs-email`.

### Tasks - Verification & Testing

- [x] Write integration and unit tests for backend `EmailAuditLogsController`.
- [x] Write Vitest unit tests for frontend `EmailAuditLogService` and logs list component.
- [x] Run `npm run test` to verify all tests compile and pass.
>  ✅ 2026-06-30 18:36 - Created EmailAuditLogQueryTests.cs in backend, email-audit-log.service.spec.ts in frontend, and ran build/tests cleanly.

---

## Expected Outcome

1. Backend endpoint (`EmailAuditLogsController`) exposed in Scalar/OpenAPI.
2. Premium paginated logs dashboard showing email audit history.
3. Details modal for inspecting delivery errors and failures in BR-Portuguese.
4. Clean and covered unit tests under Vitest.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
