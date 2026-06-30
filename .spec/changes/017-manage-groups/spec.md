# 017-manage-groups

## Objective

Implement the Group management UI for Group Leaders. This allows Group Leaders to build groups by aggregating partners, a client company, a project, client contacts, group timezone settings, and scheduling report rules.

## Technical Context

Since Group management endpoints are not yet fully exposed on the Presentation layer, this specification includes both the backend exposing steps and the frontend UI implementation steps.
- Align with: [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md) and backend playbooks.
- Backend: Expose a REST controller and command/query handlers.
- Frontend: Standalone Angular 22 components utilizing Signals and property-level DI.

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, labels, and validations must be in **BR-Portuguese** (RULE-014).
- The timezone field select control must retrieve and validate timezones against system timezones (e.g. `America/Sao_Paulo`).
- Access to group creation is restricted to the `GroupLeader` and `Manager` roles.

---

## Tasks

### Tasks - Backend Layer (`GreatReports.Presentation` & `Application`)

- [ ] Expose query handlers for listing groups (e.g., `GetGroupsQuery`, `GetGroupByIdQuery`) in `GreatReports.Application`.
- [ ] Create `GroupsController` (`src/backend/GreatReports.Presentation/Controllers/GroupsController.cs`):
  - Decorate with `[Authorize(Roles = "GroupLeader,Manager")]`.
  - Expose `POST /api/Groups` (binding `CreateGroupCommand` and returning `ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)`).
  - Expose `GET /api/Groups` and `GET /api/Groups/{id}`.
- [ ] Regenerate the OpenAPI frontend client (`npm run generate:api`).

### Tasks - Core Services Wrapper

- [ ] Create core wrapper service `src/frontend/src/app/core/services/group.service.ts`:
  - Expose `createGroup(req: CreateGroupRequest): Promise<string>`.
  - Expose `getGroups(): Promise<GroupDto[]>`.

### Tasks - Group UI Components

- [ ] Create standalone GroupListComponent (`src/frontend/src/app/features/group-leader/group-list/group-list.component.ts`):
  - Grid showing groups owned by the Group Leader or all groups for Managers.
- [ ] Create standalone GroupRegisterComponent (`src/frontend/src/app/features/group-leader/group-register/group-register.component.ts`):
  - Multi-select form to tie Client Company, Project, Partners, Timezone, and Scheduled Reports.

### Tasks - Routing Setup

- [ ] Wire lazy routing in `src/app/app.routes.ts`:
  - `/lider/grupos` -> GroupListComponent
  - `/lider/grupos/novo` -> GroupRegisterComponent

### Tasks - Verification & Testing

- [ ] Write integration and unit tests for backend `GroupsController` and use cases.
- [ ] Write Vitest unit tests for frontend `GroupService` and group components.
- [ ] Run `npm run test` to verify all tests compile and pass.

---

## Expected Outcome

1. Backend controller (`GroupsController`) successfully mapped in Scalar/OpenAPI.
2. Premium data grid and group creation form in BR-Portuguese.
3. Access restricted to GroupLeaders and Managers.
4. Full Vitest unit test coverage.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
