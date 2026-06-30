---
name: frontend-framework-definitions
description: Guidelines for front-end TypeScript, Angular core rules, reactive signals, state management, API service wrapping, and Tailwind CSS.
---

# Skill: TS/Angular Core Rules and Styling

You are an expert in TypeScript, Angular, and scalable web application development. You write functional, maintainable, performant, and accessible code following Angular and TypeScript best practices.

---

## TS/Angular Core Rules

- **Strict Type Checking**: Must be enabled. Avoid using `any`; use `unknown` or explicit interfaces defined in `core/models/`.
- **Standalone Components**: Always use standalone components. Do NOT set `standalone: true` in component decorators (default in Angular v20+).
- **Control Flow**: Always use native control flow (`@if`, `@for`, `@switch`) instead of legacy `*ngIf`/`*ngFor`.
- **Dependency Injection**: Use the `inject()` function at the property level instead of constructor injection.
  ```typescript
  private readonly http = inject(HttpClient);
  ```

---

## State Management with Signals
- **Component State**: Use Signals (`signal()`, `computed()`) for local component and service state management.
- **Inputs & Outputs**: Use the `input()` and `output()` functions instead of legacy decorators (`@Input`, `@Output`).
  ```typescript
  readonly itemId = input.required<string>();
  readonly itemChanged = output<void>();
  ```
- **Derived State**: Always use `computed()` to calculate derived state rather than manual recalculation.
- **State Mutation**: Do NOT use `mutate` on signals. Always use `update()` or `set()`.

---

## API Services and HTTP Communication
- **Service Registration**: Services must be singleton and use the `providedIn: 'root'` option.
- **Asynchronous Actions**: For transactional requests (e.g. login, submit forms), prefer converting HTTP Observables to Promises using RxJS `firstValueFrom` to allow clean async/await patterns.
  ```typescript
  async submitData(data: CreateRequest): Promise<string> {
    return await firstValueFrom(
      this.http.post<string>(this.apiBase, data)
    );
  }
  ```
- **Queries/Lists**: Standard read streams can return `Observable<T>` directly. Use the async pipe or `.subscribe` to populate signals.
- **Generated Clients vs Manual Services**: Wrap generated API clients into hand-written services that map directly to the clean models in `core/models/` for simpler component interfaces.

---

## Tailwind CSS Theme Styling Standards
- **Theme Variables**: Enforce the usage of theme colors and design tokens defined in the `@theme` block of the main CSS sheet.
  - Backgrounds: `bg-bg-primary`, `bg-bg-secondary`, `bg-bg-dark`
  - Text: `text-text-primary`, `text-text-secondary`, `text-accent-brand`
- **Transitions**: Use smooth transitions for hover effects: `transition-all duration-400 ease-[cubic-bezier(0.16,1,0.3,1)]`.
- **No Arbitrary Colors**: Do not hardcode arbitrary hexadecimal or RGB colors directly in templates (avoid `bg-[#c5a880]`). Use theme tokens instead.
