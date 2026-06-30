# 010-scaffold-frontend

## Objective

Scaffold the frontend project by configuring the API client generation, unit testing via Vitest, Tailwind CSS v4 design system, global HTTP interceptors, and implementing the authentication UI components (Login, Register, Confirm Email, Forgot Password, Reset Password) matching the visual and structural standards of Great Reports.

## Technical Context

We are scaffolding the frontend project located under `src/frontend/`. We need to align the project with the definitions in [Global Technical Context](../../memory/technical-context.md) and the skills:
- [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md)
- [10-frontend-framework-definitions](../../../.gemini/skills/10-frontend-framework-definitions/SKILL.md)
- [11-frontend-admin-design-system](../../../.gemini/skills/11-frontend-admin-design-system/SKILL.md)

We will use:
- **Angular 22** with standalone components, Signals, and property-level dependency injection.
- **ng-openapi-gen** to generate client APIs from the backend Scalar/OpenAPI definition.
- **Vitest** for running unit tests.
- **Tailwind CSS v4** with a custom dark-theme glassmorphism configuration.

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, validation feedback, and alerts must be in **BR-Portuguese** (RULE-014).
- Do not add `standalone: true` or `changeDetection: ChangeDetectionStrategy.OnPush` to component decorators (defaults in Angular 22).
- Use `firstValueFrom` in core wrapper services to map RxJS Observables to Promises to allow async/await workflows in components.
- The focus states must avoid default browser rings; use the custom design system class: `focus-visible:ring-2 focus-visible:ring-accent-brand focus-visible:ring-offset-2 focus-visible:ring-offset-bg-primary focus-visible:outline-none`.

---

## Tasks

### Tasks - Configuration

- [ ] Install `ng-openapi-gen` and `vitest` as devDependencies:
  - `npm install -D ng-openapi-gen vitest @angular/build` (verify if already present/resolved).
- [ ] Add the following scripts to `src/frontend/package.json`:
  ```json
  "generate:api": "ng-openapi-gen -i openapi.json -o src/app/api",
  "test": "vitest run"
  ```
- [ ] Create `src/frontend/ng-openapi-gen.json` to customize output path and disable deprecated elements:
  ```json
  {
    "$schema": "./node_modules/ng-openapi-gen/ng-openapi-gen-schema.json",
    "input": "openapi.json",
    "output": "src/app/api",
    "ignoreUnusedModels": false
  }
  ```
- [ ] Create `src/frontend/vitest.config.ts` to configure Vitest with Angular testing support:
  ```typescript
  import { defineConfig } from 'vitest/config';
  import angular from '@analogjs/vite-plugin-angular';

  export default defineConfig({
    plugins: [angular()],
    test: {
      globals: true,
      environment: 'jsdom',
      setupFiles: ['src/test-setup.ts'],
      include: ['src/**/*.spec.ts'],
    },
  });
  ```
- [ ] Create `src/frontend/src/test-setup.ts` to initialize Angular testing environment for Vitest:
  ```typescript
  import '@analogjs/vite-plugin-angular/setup-vitest';
  import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
  import { TestBed } from '@angular/core/testing';

  TestBed.initTestEnvironment(
    BrowserDynamicTestingModule,
    platformBrowserDynamicTesting()
  );
  ```

### Tasks - API Client & Core Services Wrapper

- [ ] Fetch the current backend OpenAPI schema from `http://localhost:5147/openapi/v1.json` and save it to `src/frontend/openapi.json`.
- [ ] Generate the client services and models:
  - Run `npm run generate:api` inside `src/frontend/`.
- [ ] Implement core wrapper service `src/frontend/src/app/core/services/auth.service.ts`:
  - Property-inject the generated `AuthApiService` (or equivalent endpoint client) using `inject()`.
  - Expose promise-based methods utilizing `firstValueFrom`.
  - Expose signals for `currentUser` and `isAuthenticated` representing the current session.
  - Implement basic JWT storage in `localStorage` or `sessionStorage`.
- [ ] Implement core wrapper service `src/frontend/src/app/core/services/company.service.ts` wrapping client/provider company queries.
- [ ] Implement core wrapper service `src/frontend/src/app/core/services/user.service.ts` wrapping user details and activation.

### Tasks - Frontend Shell & Styling

- [ ] Update `src/frontend/src/styles.css` to define the Tailwind CSS v4 `@theme` configuration:
  - Add colors: `--color-bg-primary` (#0B0F19), `--color-bg-secondary` (#111827), `--color-bg-tertiary` (#1F2937).
  - Add borders: `--color-border-glass` (rgba(255, 255, 255, 0.08)), `--color-border-hover` (rgba(255, 255, 255, 0.15)).
  - Add accents: `--color-accent-brand` (#4F46E5), `--color-accent-emerald` (#10B981), `--color-accent-rose` (#F43F5E), `--color-accent-amber` (#F59E0B).
  - Add text: `--color-text-primary` (#FFFFFF), `--color-text-secondary` (#94A3B8), `--color-text-dim` (#64748B).
  - Configure fonts: `Outfit` for headings, `Inter` for controls/body, and `JetBrains Mono` for IDs/numeric text.
- [ ] Include Google Fonts preconnect and links in `src/frontend/public/index.html`.
- [ ] Implement an HTTP Interceptor `src/frontend/src/app/core/interceptors/auth.interceptor.ts` (functional style) to attach JWT Bearer tokens to all outbound API requests.

### Tasks - Frontend UI Components (Auth Feature)

- [ ] Create LoginComponent (`src/frontend/src/app/features/auth/login/login.component.ts`):
  - Form validation for email and password.
  - Dark glassmorphic container layout.
  - Expose login request mapping and redirect to dashboard upon success.
- [ ] Create RegisterComponent (`src/frontend/src/app/features/auth/register/register.component.ts`):
  - Form for full registration fields.
  - Atomic validation checks.
- [ ] Create ConfirmEmailComponent (`src/frontend/src/app/features/auth/confirm-email/confirm-email.component.ts`):
  - Handshake using route query parameters (`userId` and `token`).
  - Render confirmation success/error status messages in BR-Portuguese.
- [ ] Create ForgotPasswordComponent (`src/frontend/src/app/features/auth/forgot-password/forgot-password.component.ts`):
  - Simple form requesting user's email to trigger the recovery notification.
- [ ] Create ResetPasswordComponent (`src/frontend/src/app/features/auth/reset-password/reset-password.component.ts`):
  - Form requiring password confirmation and passing the token extracted from the URL.
- [ ] Wire routing in `src/frontend/src/app/app.routes.ts`:
  - Add lazy-loaded child routes for `/auth/login`, `/auth/register`, `/auth/confirm-email`, `/auth/forgot-password`, `/auth/reset-password`.

### Tasks - Verification & Testing

- [ ] Create unit tests for `AuthService` (`src/frontend/src/app/core/services/auth.service.spec.ts`) using Vitest:
  - Verify token retention.
  - Verify error mapping and state update behavior.
- [ ] Create unit tests for `LoginComponent` (`src/frontend/src/app/features/auth/login/login.component.spec.ts`) using Vitest:
  - Mock `AuthService` and route redirects.
  - Verify validation state blocks submit.
- [ ] Verify accessibility (A11y) of authentication forms.
- [ ] Run `npm run test` inside the frontend to verify all tests compile and pass.

---

## Expected Outcome

1. Build system updated to support `ng-openapi-gen` client generation and `Vitest` unit tests.
2. Styling system configured with custom dark slate, glassmorphism, and premium typography settings in Tailwind CSS v4.
3. Functional HTTP API Client generated and wrapped within type-safe Promise-based services.
4. Working login, registration, password recovery, and email verification pages with complete user flows.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
