# 016-manage-projects

## Objective

Implement the Project frontend registration interface. This enables Managers to configure and register new projects associated with a Client Company.

## Technical Context

We will build upon the core frontend scaffolding. We need to align the implementation with:
- [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md)
- [10-frontend-framework-definitions](../../../.gemini/skills/10-frontend-framework-definitions/SKILL.md)
- [11-frontend-admin-design-system](../../../.gemini/skills/11-frontend-admin-design-system/SKILL.md)

We will use:
- **Angular 22** standalone components, Signals, and property-level dependency injection (`inject()`).
- **CompanyService** under `src/app/core/services/` to wrap `apiProjectsPost`.
- **Tailwind CSS v4** design variables for registration forms.
- The backend projects endpoint (`/api/Projects`).

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, error validations, labels, and success feedback must be in **BR-Portuguese** (RULE-014).
- The Project registration form must show a dropdown select of available Client Companies.
- Display a list of projects in the administrative panel.

---

## Tasks

### Tasks - Core Services Wrapper

- [x] Modify `src/app/core/services/company.service.ts` to wrap `apiProjectsPost` using the generated client.
  - Expose `registerProject(req: RegisterProjectRequest): Promise<string>`.
>  ✅ 2026-06-29 14:10 - Implemented registerProject in CompanyService.

### Tasks - Project UI Components

- [x] Create standalone ProjectRegisterComponent (`src/app/features/admin/project-register/project-register.component.ts`):
  - Form validations for Project Name and Client Company selection.
  - Bind submit handler invoking `CompanyService.registerProject`.
  - Handle success/error feedback in BR-Portuguese.
>  ✅ 2026-06-29 14:10 - Created ProjectRegisterComponent.
- [x] Create standalone ProjectListComponent (`src/app/features/admin/project-list/project-list.component.ts`) displaying projects.
>  ✅ 2026-06-29 14:10 - Created ProjectListComponent.

### Tasks - Routing Setup

- [x] Wire routing in `src/app/app.routes.ts`:
  - `/admin/projetos` -> ProjectListComponent
  - `/admin/projetos/novo` -> ProjectRegisterComponent
>  ✅ 2026-06-29 14:10 - Configured routes.

### Tasks - Verification & Testing

- [x] Write Vitest unit tests for `CompanyService` project operations.
>  ✅ 2026-06-29 14:10 - Wrote CompanyService project test specs.
- [x] Write Vitest unit tests for `ProjectRegisterComponent`.
>  ✅ 2026-06-29 14:10 - Wrote component test specs.
- [x] Run `npm run test` to verify all tests compile and pass.
>  ✅ 2026-06-29 14:10 - Verified all tests pass.

---

## Expected Outcome

1. A project list grid displaying registered projects.
2. A functional project registration form.
3. Access restricted to the `Manager` role.
4. Clean and covered unit tests under Vitest.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
