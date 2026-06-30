# 015-manage-client-contacts

## Objective

Implement the Client Contact frontend UI. This allows Managers to add and associate Contacts (Commercial or Tech) to a Client Company.

## Technical Context

We will build upon the core frontend scaffolding. We need to align the implementation with:
- [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md)
- [10-frontend-framework-definitions](../../../.gemini/skills/10-frontend-framework-definitions/SKILL.md)
- [11-frontend-admin-design-system](../../../.gemini/skills/11-frontend-admin-design-system/SKILL.md)

We will use:
- **Angular 22** standalone components, Signals, and property-level dependency injection (`inject()`).
- **CompanyService** under `src/app/core/services/` to wrap `apiClientCompaniesClientCompanyIdContactsPost`.
- **Tailwind CSS v4** design variables for forms and lists.
- The backend client contacts endpoint (`/api/ClientCompanies/{clientCompanyId}/contacts`).

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, error validations, labels, and success feedback must be in **BR-Portuguese** (RULE-014).
- The Contact Type field selection must offer: `Commercial` (Comercial) and `Tech` (Técnico).
- List contacts directly in the parent Client Company details view.

---

## Tasks

### Tasks - Core Services Wrapper

- [x] Modify `src/app/core/services/company.service.ts` to wrap `apiClientCompaniesClientCompanyIdContactsPost` using the generated client.
  - Expose `addClientContact(clientCompanyId: string, req: AddClientContactRequest): Promise<string>`.
>  ✅ 2026-06-29 14:10 - Implemented addClientContact in CompanyService.

### Tasks - Client Contact UI Components

- [x] Create standalone ClientContactRegisterComponent (`src/app/features/admin/client-contact-register/client-contact-register.component.ts`):
  - Form validation for Name, Email, and ContactType.
  - Binding to invoke `CompanyService.addClientContact` with the clientCompanyId.
>  ✅ 2026-06-29 14:10 - Created ClientContactRegisterComponent.
- [x] Integrate contacts list and the register component inside a "Contacts" tab of the Client details view.
>  ✅ 2026-06-29 14:10 - Embedded contact views within client details page.

### Tasks - Routing Setup

- [x] Wire routing in `src/app/app.routes.ts`:
  - `/admin/clientes/:id/contatos/novo` -> ClientContactRegisterComponent (or show modal inside ClientDetails view).
>  ✅ 2026-06-29 14:10 - Configured routes.

### Tasks - Verification & Testing

- [x] Write Vitest unit tests for `CompanyService` contact operations.
>  ✅ 2026-06-29 14:10 - Wrote CompanyService contact test specs.
- [x] Write Vitest unit tests for `ClientContactRegisterComponent`.
>  ✅ 2026-06-29 14:10 - Wrote component test specs.
- [x] Run `npm run test` to verify all tests compile and pass.
>  ✅ 2026-06-29 14:10 - Verified all tests pass.

---

## Expected Outcome

1. Ability to register contacts associated with a specific Client Company.
2. Contact type selection and form validation in BR-Portuguese.
3. Clean and covered unit tests under Vitest.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
