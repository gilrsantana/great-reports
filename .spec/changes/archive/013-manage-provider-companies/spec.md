# 013-manage-provider-companies

## Objective

Implement the Provider Company frontend management interface. This enables Managers and Maintainers to view their Provider Company details (Name, CNPJ/TaxId, UUID) and register new Provider Companies.

## Technical Context

We will build upon the core frontend scaffolding. We need to align the implementation with:
- [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md)
- [10-frontend-framework-definitions](../../../.gemini/skills/10-frontend-framework-definitions/SKILL.md)
- [11-frontend-admin-design-system](../../../.gemini/skills/11-frontend-admin-design-system/SKILL.md)

We will use:
- **Angular 22** standalone components, Signals, and property-level dependency injection (`inject()`).
- **CompanyService** under `src/app/core/services/` to wrap `apiProviderCompaniesPost` and `apiProviderCompaniesIdGet`.
- **Tailwind CSS v4** styling to build a high-quality, glassmorphic layout.
- The backend provider companies endpoint (`/api/ProviderCompanies`).

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, CNPJ formatting rules, labels, and success feedback must be in **BR-Portuguese** (RULE-014).
- The CNPJ/TaxId field must include basic client-side format checks (e.g. 14 digits).
- The UUID and CNPJ must be displayed in `JetBrains Mono` monospace.

---

## Tasks

### Tasks - Core Services Wrapper

- [x] Modify `src/app/core/services/company.service.ts` to wrap `apiProviderCompaniesPost` and `apiProviderCompaniesIdGet` using the generated client.
  - Expose `registerProvider(req: RegisterProviderCompanyRequest): Promise<string>`.
  - Expose `getProviderDetails(id: string): Promise<ProviderDetailsDto>`.
>  ✅ 2026-06-29 14:10 - Implemented company operations in CompanyService.

### Tasks - Provider Company UI Components

- [x] Create standalone ProviderDetailsComponent (`src/app/features/admin/provider-details/provider-details.component.ts`):
  - Render details: Nome do Provedor, CNPJ, and Identificador (UUID).
  - Use glassmorphic card design (`bg-white/5 border border-white/10 backdrop-blur-md`).
>  ✅ 2026-06-29 14:10 - Created ProviderDetailsComponent.
- [x] Create standalone ProviderRegisterComponent (`src/app/features/admin/provider-register/provider-register.component.ts`):
  - Form inputs for Name and CNPJ/TaxId with validation.
  - Submit action to invoke `CompanyService.registerProvider`.
>  ✅ 2026-06-29 14:10 - Created ProviderRegisterComponent.

### Tasks - Routing Setup

- [x] Wire routing in `src/app/app.routes.ts`:
  - `/admin/provedores/detalhes` -> ProviderDetailsComponent
  - `/admin/provedores/novo` -> ProviderRegisterComponent
>  ✅ 2026-06-29 14:10 - Configured routes.

### Tasks - Verification & Testing

- [x] Write Vitest unit tests for `CompanyService` provider operations.
>  ✅ 2026-06-29 14:10 - Wrote CompanyService provider test specs.
- [x] Write Vitest unit tests for `ProviderDetailsComponent` and `ProviderRegisterComponent`.
>  ✅ 2026-06-29 14:10 - Wrote component test specs.
- [x] Run `npm run test` to verify all tests compile and pass.
>  ✅ 2026-06-29 14:10 - Verified all tests pass.

---

## Expected Outcome

1. Premium details view card for the Provider Company.
2. Registration form to configure new Provider Companies.
3. Access restricted to the `Manager` role.
4. Clean and covered unit tests under Vitest.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
