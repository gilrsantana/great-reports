# 021-frontend-admin-design-system

## Objective

Standardize the frontend application's visual architecture and page layouts by implementing the premium dark-theme admin design system. This includes configuring the global typography, theme colors, layout elements, custom scrollbars, and ensuring all components conform to Angular 22 framework guidelines, Vitest unit testing, and accessibility (A11y) standards.

## Technical Context

We are standardizing the look, feel, and structural constraints of the frontend project located under `src/frontend/`. We align this change with:
- [00-frontend-scaffold-playbook](../../../.gemini/skills/00-frontend-scaffold-playbook/SKILL.md)
- [10-frontend-framework-definitions](../../../.gemini/skills/10-frontend-framework-definitions/SKILL.md)
- [11-frontend-admin-design-system](../../../.gemini/skills/11-frontend-admin-design-system/SKILL.md)

All existing admin pages (`admin-dashboard`, `user-list`, `client-list`, `project-list`, etc.) will be refactored or updated to inherit the shared layout structure, styling theme, typography classes, custom focus rings, and proper a11y attributes.

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- All user-facing UI text, buttons, alerts, and placeholders must remain in **BR-Portuguese** (RULE-014).
- Use Tailwind CSS v4 variables (e.g. `--color-bg-primary`, `--color-accent-brand`) instead of arbitrary colors.
- Custom outline rings for focus states are mandatory: `focus-visible:ring-2 focus-visible:ring-accent-brand focus-visible:ring-offset-2 focus-visible:ring-offset-bg-primary focus-visible:outline-none`.
- Do not define `standalone: true` or `changeDetection: ChangeDetectionStrategy.OnPush` in component decorators (these are defaults).

---

## Tasks

### Tasks - Global Styles & Assets

- [ ] Verify that `src/frontend/src/index.html` imports Google Fonts (`Outfit`, `Inter`, `JetBrains Mono`).
- [ ] Verify that `src/frontend/src/styles.css` defines the Tailwind CSS v4 `@theme` variables (Surfaces, Borders, Accents, Text, and Transitions) and custom scrollbar rules.
- [ ] Verify that standard container styling helpers (like dark glassmorphism and interactive cards) are declared in `styles.css` if needed globally.

### Tasks - Shell Layout Component

- [ ] Create or update a main Admin layout/shell component or wrapper to structure:
  - Left fixed Sidebar containing the navigation options (Painel, Usuários, Clientes, Projetos, Logs de E-mail) with active route indicator states.
  - Header area displaying context ("Painel Administrativo" / current user profile or role).
  - Central scrollable `<main>` area hosting the `<router-outlet>`.
- [ ] Update routes in `src/frontend/src/app/app.routes.ts` to host admin components under this layout if applicable, or ensure they integrate the layout structure directly.

### Tasks - Refactoring UI Components to Design System

Apply design tokens (fonts, colors, hover transitions, layouts, grid/table styling) to the following components:
- [ ] **Admin Dashboard**: `src/frontend/src/app/features/admin/admin-dashboard.component.html`
  - Use `Outfit` for primary headings and metric values.
  - Structure KPI card metrics inside a grid with modern border-glass colors and emerald/rose micro-trends.
  - Use `JetBrains Mono` for CNPJs, IDs, and numeric records.
- [ ] **Data Tables**:
  - Refactor client tables, user tables, and project tables to use `divide-white/5`, `hover:bg-white/5` row hover states, and monospace text for UUIDs.
- [ ] **Forms**: `src/frontend/src/app/features/admin/entity-form.component.html`
  - Standardize labels, text inputs, and select fields with proper background surfaces (`--color-bg-tertiary` or `bg-white/5`), border tokens, and focus rings.

### Tasks - Verification & Testing

- [ ] Ensure all existing frontend unit tests pass by running:
  ```bash
  npm run test
  ```
- [ ] Ensure that Vitest configuration runs successfully.

### Tasks - Accessibility (A11y)

- [ ] Confirm all tables use proper table semantic headers (`<th>` with scope attributes if required).
- [ ] Confirm all input controls have an associated `<label>` or `aria-label`.
- [ ] Verify that focus states are visible and navigable via keyboard (Tab key) on all admin dashboards and modal dialogs.
- [ ] Verify that dark-theme text contrast meets WCAG AA guidelines.

---

## Expected Outcome

- A unified, premium look and feel for the Great Reports admin portal based on dark-theme aesthetics.
- Seamless responsive navigation shell using the new layout components.
- Cleaner component templates strictly adhering to Angular 22 standalone, signals, and Tailwind CSS v4 guidelines.
- 100% passing tests and fully compliant accessibility attributes across all refactored views.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
