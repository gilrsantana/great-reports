---
name: frontend-framework-definitions
description: Declarative rules and architecture constraints for front-end TypeScript, Angular core elements, reactive signals, API-first generation, forms, and accessibility.
---

# Skill: TS/Angular Core Rules and Constraints

This skill defines the strict architectural constraints, syntax rules, and standards that all frontend code in Great Reports must satisfy.

---

## 1. TypeScript Rules
- **Strict Type Checking**: Must be enabled at all times.
- **Explicit vs Inferred**: Prefer type inference when the type is obvious (e.g. `readonly flag = signal(false)`). Otherwise, define type interfaces explicitly.
- **No `any`**: The use of the `any` type is strictly forbidden. Use `unknown` or declare a strong interface.
- **Model Storage**: All custom frontend domain interfaces must reside in `src/app/core/models/` or be generated from `openapi.json`.

---

## 2. Angular Component Constraints
- **Standalone Components**: All components must be standalone. Do NOT define `standalone: true` in component decorators (this is the default in Angular v20+).
- **Change Detection**: Do NOT set `changeDetection: ChangeDetectionStrategy.OnPush` inside component decorators. `OnPush` is the default in Angular v22+.
- **Host Bindings**: The `@HostBinding` and `@HostListener` decorators are deprecated. You MUST put host bindings inside the `host` metadata property of the component/directive decorator:
  ```typescript
  @Component({
    selector: 'app-btn',
    host: {
      'class': 'btn-base',
      '[class.disabled]': 'isDisabled()',
      '(click)': 'onBtnClick()'
    }
  })
  ```
- **Control Flow**: Always use native control flow block syntax (`@if`, `@for`, `@switch`) instead of legacy structural directives (`*ngIf`, `*ngFor`, `*ngSwitch`).
- **Template Variables**: Do not assume globals (e.g. `new Date()`) are available directly in templates.
- **Class and Style Bindings**: The `ngClass` and `ngStyle` directives are deprecated. Use class binding (`[class.active]="isActive()"`) and style binding (`[style.color]="color()"`) instead.
- **Image Optimization**: Use `NgOptimizedImage` for all static images (excluding inline base64 images).

---

## 3. Dependency Injection (DI)
- **Property-Level injection**: Use the `inject()` function at the property level instead of constructor injection:
  ```typescript
  // YES
  private readonly http = inject(HttpClient);

  // NO
  constructor(private http: HttpClient) {}
  ```
- **Service Registration**: Singleton services must use the `providedIn: 'root'` option.
- **Decorator Choice**: Prefer the `@Service` decorator over `@Injectable({providedIn: 'root'})` for new singleton services (Angular v22+).

---

## 4. State Management with Signals
- **State Definition**: Use Signals (`signal()`, `computed()`) for local component and service state.
- **Inputs & Outputs**: Use `input()`, `input.required()`, and `output()` functions instead of legacy decorators (`@Input`, `@Output`):
  ```typescript
  readonly id = input.required<string>();
  readonly clickEvent = output<void>();
  ```
- **Derived State**: Always calculate derived values using `computed()`. Do not recalculate manually in templates or logic.
- **No Direct Mutation**: Never call `mutate()` on signals. Use `set()` or `update()`.

---

## 5. API First Design with `ng-openapi-gen`
- **Zero Handcrafted HTTP Requests**: Do NOT write custom REST endpoint request services manually.
- **Code Generation**: All API interaction services and data transfer models must be generated directly from `openapi.json` using `ng-openapi-gen`.
- **Wrapper Services**: Wrap generated API clients into hand-written services under `src/app/core/services/` to simplify component integrations, map domain models, or cache results.
- **Asynchronous Mappings**: Standardize on `firstValueFrom` to map Observables to Promises when performing operations/actions inside components that benefit from async/await.

---

## 6. Forms Standards
- **Preferred Option**: Use Signal Forms (`@angular/forms/signals`) for all new forms (stable in Angular v22+). This provides signal-based state, type-safety, and schema validation.
- **Fallback Option**: Use Reactive Forms instead of Template-driven ones if Signal Forms are not suitable for the use case.

---

## 7. Accessibility (A11y)
- **Compliance Level**: All UI features must meet the WCAG AA accessibility minimums (proper focus management, contrast, and ARIA labels).
- **Axe Auditing**: No code should be committed if it fails AXE accessibility verification.

---

## 8. Unit Testing Standards
- **Test Runner**: Vitest (globals configured via `tsconfig.spec.json`).
- **Mandatory Spec Files**: Every feature service and component MUST have a corresponding `.spec.ts` unit test file.
- **Isolate Components**: Mock all injected services inside `TestBed` configuration using Vitest spy utilities (`vi.fn()`, custom providers).
- **Signal Mappings**: Test signal inputs/outputs by simulating changes in state or triggers and asserting on derived values.
- **Service Isolation**: Mock generated API service clients. Test mapping layers, error transformations, and `firstValueFrom` promise resolutions using resolved/rejected mock values.

