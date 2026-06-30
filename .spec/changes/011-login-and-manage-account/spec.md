# 011-login-and-manage-account

## Objective

Implement the Login UI page and the Account Management section for logged-in users. This allows users to log in securely, view their current profile details, and update their password, using Angular 22 standalone components, reactive Signals, and custom Tailwind CSS v4 glassmorphic styling.

## Technical Context

We will build upon the frontend scaffolding. We need to align the implementation with:
- [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md)
- [10-frontend-framework-definitions](../../../.gemini/skills/10-frontend-framework-definitions/SKILL.md)
- [11-frontend-admin-design-system](../../../.gemini/skills/11-frontend-admin-design-system/SKILL.md)

We will use:
- **Angular 22** standalone components, Signals, and property-level dependency injection (`inject()`).
- **firstValueFrom** to resolve generated API client Observables as Promises.
- **Tailwind CSS v4** design system variables (colors, fonts, borders, motion).
- The backend authentication endpoints (`/api/Auth/login` and `/api/Auth/change-password`).

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, error validations, labels, and success feedback must be in **BR-Portuguese** (RULE-014).
- Protect the `/minha-conta` route using a functional route guard (`authGuard`) that redirects unauthenticated users to `/login`.
- Manage session tokens (`accessToken` and `refreshToken`) securely in `localStorage` or `sessionStorage` and propagate them via a functional HTTP interceptor.
- The focus states must avoid default browser rings; use the custom design system class: `focus-visible:ring-2 focus-visible:ring-accent-brand focus-visible:ring-offset-2 focus-visible:ring-offset-bg-primary focus-visible:outline-none`.

---

## Tasks

### Tasks - Core Services Wrapper

- [ ] Implement token storage and session signals in `src/app/core/services/auth.service.ts`:
  - Expose signals: `currentUser` (containing decoded claims or profile info) and `isAuthenticated`.
  - Implement token retention and automated loading on startup.
- [ ] Implement `login` method in `AuthService` calling `apiAuthLoginPost`:
  - Pass credentials, map the returned `TokenResponse`, store the access and refresh tokens, and update session signals.
- [ ] Implement `changePassword` method in `AuthService` calling `apiAuthChangePasswordPost`:
  - Inject HTTP context or verify authorization.
- [ ] Implement `logout` method in `AuthService` to clear tokens and reset signals.

### Tasks - Route Guard & Interceptors

- [ ] Implement functional guard `src/app/core/guards/auth.guard.ts`:
  - Verify if `AuthService.isAuthenticated()` is true; if not, redirect to `/login`.
- [ ] Implement functional HTTP interceptor `src/app/core/interceptors/auth.interceptor.ts`:
  - If a JWT token is stored, clone the request and append the `Authorization: Bearer <token>` header.
  - Optionally handle token refreshing on `401 Unauthorized` responses.

### Tasks - Login UI Component

- [ ] Create standalone LoginComponent (`src/app/features/auth/login/login.component.ts`):
  - Form validation for email format and password length (min 8 characters).
  - Centered card using dark glassmorphic styling (`bg-white/5 border border-white/10 backdrop-blur-md`).
  - Integrate submit handler to invoke `AuthService.login`, display a loading indicator, and handle errors in BR-Portuguese.
- [ ] Implement component template `login.component.html` and styles `login.component.css` in BR-Portuguese.

### Tasks - Account Management UI Component

- [ ] Create standalone AccountManagementComponent (`src/app/features/account/account-management/account-management.component.ts`):
  - Router path: `/minha-conta` (guarded by `authGuard`).
  - Integrate main admin layout shell (Sidebar and Top Header).
  - Render read-only details of the logged-in user (Nome de Exibição, E-mail, Função) in BR-Portuguese.
  - Implement form to update password (Current Password, New Password, Confirm New Password) with frontend validation checking that new password fields match.
  - Implement submit handler invoking `AuthService.changePassword` and showing success/error feedback in BR-Portuguese.
- [ ] Implement templates and styles matching the custom Slate/Glassmorphism theme.

### Tasks - Routing Setup

- [ ] Wire lazy-loaded routes in `src/app/app.routes.ts`:
  - `/login` -> LoginComponent
  - `/minha-conta` -> AccountManagementComponent (guarded by `authGuard`)

### Tasks - Verification & Testing

- [ ] Write Vitest unit tests for `AuthService` (`src/app/core/services/auth.service.spec.ts`):
  - Test login success/failure, token storage, and logout.
- [ ] Write Vitest unit tests for `LoginComponent` (`src/app/features/auth/login/login.component.spec.ts`):
  - Test validation states and form submit redirection.
- [ ] Write Vitest unit tests for `AccountManagementComponent` (`src/app/features/account/account-management/account-management.component.spec.ts`):
  - Test profile details rendering and password change submission.
- [ ] Run `npm run test` inside the frontend to verify all tests compile and pass.

---

## Expected Outcome

1. Secure login interface with robust validation, loading states, and error handling in BR-Portuguese.
2. Account Management section showing user details and providing password change functionality.
3. Client-side route protection ensuring only authenticated users can access the profile area.
4. Comprehensive Vitest coverage verifying authentication flows and profile operations.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
