# 014-manage-client-companies

## Objective

Implement the Client Company frontend management interface. This enables Managers to register Client Companies and view a paginated list of client companies associated with their Provider Company.

## Technical Context

We will build upon the core frontend scaffolding. We need to align the implementation with:
- [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md)
- [10-frontend-framework-definitions](../../../.gemini/skills/10-frontend-framework-definitions/SKILL.md)
- [11-frontend-admin-design-system](../../../.gemini/skills/11-frontend-admin-design-system/SKILL.md)

We will use:
- **Angular 22** standalone components, Signals, and property-level dependency injection (`inject()`).
- **CompanyService` under `src/app/core/services/` wrapping `apiClientCompaniesPost` and `apiClientCompaniesGet`.
- **Tailwind CSS v4** styling to build a premium, glassmorphic layout with paginated data tables.
- The backend client companies endpoint (`/api/ClientCompanies`).

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, error validations, labels, and success feedback must be in **BR-Portuguese** (RULE-014).
- The table must support pagination (page index and page size configuration) and render the client company's Name and UUID in BR-Portuguese.
- Displays should be responsive, applying glassmorphic shadows on hover.

---

## Tasks

### Tasks - Core Services Wrapper

- [ ] Modify `src/app/core/services/company.service.ts` to wrap `apiClientCompaniesPost` and `apiClientCompaniesGet` using the generated client.
  - Expose `registerClient(req: RegisterClientCompanyRequest): Promise<string>`.
  - Expose `getClientCompanies(providerId: string, page: number, pageSize: number): Promise<PagedResponse<ClientCompanyDto>>`.

### Tasks - Client Company UI Components

- [ ] Create standalone ClientListComponent (`src/app/features/admin/client-list/client-list.component.ts`):
  - Paginated table showing Client Companies (Nome and Identificador).
  - Include pagination controls (Proxima, Anterior).
- [ ] Create standalone ClientRegisterComponent (`src/app/features/admin/client-register/client-register.component.ts`):
  - Form validation for Name.
  - Submit action to invoke `CompanyService.registerClient`.

### Tasks - Routing Setup

- [ ] Wire routing in `src/app/app.routes.ts`:
  - `/admin/clientes` -> ClientListComponent
  - `/admin/clientes/novo` -> ClientRegisterComponent

### Tasks - Verification & Testing

- [ ] Write Vitest unit tests for `CompanyService` client operations.
- [ ] Write Vitest unit tests for `ClientListComponent` and `ClientRegisterComponent`.
- [ ] Run `npm run test` to verify all tests compile and pass.

---

## Expected Outcome

1. A complete list grid showing Client Companies with pagination in BR-Portuguese.
2. A functional Client Company registration form.
3. Access restricted to the `Manager` role.
4. Clean and covered unit tests under Vitest.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
