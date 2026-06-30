# 012-manage-users

## Objective

Implement the User management frontend interface for Managers. This allows managers to register new users (Partners, Group Leaders, Managers) associated with their Provider Company, and view the list of registered users.

## Technical Context

We will build upon the core frontend scaffolding. We need to align the implementation with:
- [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md)
- [10-frontend-framework-definitions](../../../.gemini/skills/10-frontend-framework-definitions/SKILL.md)
- [11-frontend-admin-design-system](../../../.gemini/skills/11-frontend-admin-design-system/SKILL.md)

We will use:
- **Angular 22** standalone components, Signals, and property-level dependency injection (`inject()`).
- **UserService** under `src/app/core/services/` to wrap `apiUsersPost`.
- **Tailwind CSS v4** design system variables for styling tables and registration forms.
- The backend users endpoint (`/api/Users`).

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, error validations, labels, and success feedback must be in **BR-Portuguese** (RULE-014).
- The user registration page must be accessible only to authenticated users with the `Manager` role.
- Display a table containing details: Nome de Exibição, E-mail, Função (Role), and the user's UUID (rendered in `JetBrains Mono` font).

---

## Tasks

### Tasks - Core Services Wrapper

- [ ] Modify `src/app/core/services/user.service.ts` to wrap `apiUsersPost` using the generated client.
  - Expose `registerUser(req: RegisterUserRequest): Promise<string>` mapping the result as a Promise using `firstValueFrom`.

### Tasks - User Management UI Component

- [ ] Create standalone UserListComponent (`src/app/features/admin/user-list/user-list.component.ts`):
  - Retrieve and display the list of users in a premium data grid.
  - Apply glassmorphism row styles and Outfit headings.
- [ ] Create standalone UserRegisterComponent (`src/app/features/admin/user-register/user-register.component.ts`):
  - Form validation for Name, Email, and Role selection.
  - Bind submit handler invoking `UserService.registerUser`.
  - Display success/error messages in BR-Portuguese.

### Tasks - Routing Setup

- [ ] Wire routing in `src/app/app.routes.ts` or child route configurations:
  - `/admin/usuarios` -> UserListComponent
  - `/admin/usuarios/novo` -> UserRegisterComponent

### Tasks - Verification & Testing

- [ ] Write Vitest unit tests for `UserService` mapping and errors.
- [ ] Write Vitest unit tests for `UserListComponent` and `UserRegisterComponent`.
- [ ] Run `npm run test` to verify all tests compile and pass.

---

## Expected Outcome

1. A complete list grid showing users of the Provider Company.
2. A functional user registration form with validation and feedback in BR-Portuguese.
3. Access restricted to the `Manager` role.
4. Clean and covered unit tests under Vitest.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
