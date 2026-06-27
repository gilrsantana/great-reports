# NNN-change-description-in-kebab-case

## Objective

A single sentence describing what this change resolves and for whom.

## Technical Context

Local stack of this change, specific constraints, and prior decisions affecting this work.
Do not repeat what is already in the [Global Technical Context](../../memory/technical-context.md) — simply reference it.

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

Rules specific to this change that do not make sense as global standards.
If a rule is repeated across multiple specs, promote it to a shared or memory file.

---

## Tasks

Group the tasks by structural area. Remove sections that do not apply to this change.

### Tasks - Business / Rules
- [ ] Define and document business invariants and rules.

### Tasks - Domain Layer (`great-reports.Domain`)
- [ ] Create domain entities inheriting from `BaseEntity`.
- [ ] Implement static factory methods (`Create()`) with validations.
- [ ] Write domain events or mutation methods.

### Tasks - Application Layer (`great-reports.Application`)
- [ ] Create commands/queries records.
- [ ] Implement hand-rolled CQRS Command/Query Handlers (`ICommandHandler` / `IQueryHandler`).
- [ ] Map domain entities to DTOs.
- [ ] Define repository and service interfaces.

### Tasks - Infrastructure Layer (`great-reports.Infrastructure`)
- [ ] Implement database configurations (`IEntityTypeConfiguration<T>`).
- [ ] Implement repositories inheriting from `BaseEntityRepository<T>`.
- [ ] Add and execute EF Core database migrations.

### Tasks - Presentation Layer (`great-reports.Presentation`)
- [ ] Create REST controllers inheriting from `ApiControllerBase`.
- [ ] Register new dependencies in composition roots (`DependencyInjection.cs`).

### Tasks - Frontend (`Angular 22`)
- [ ] Create models corresponding to DTOs in `src/app/core/models/`.
- [ ] Implement API client wrapper services in `src/app/core/services/` using `firstValueFrom`.
- [ ] Create standalone components utilizing Signals for state.
- [ ] Apply Tailwind CSS v4 styling matching custom theme tokens.
- [ ] Wire up lazy routing in `app.routes.ts`.

### Tasks - Verification & Testing
- [ ] Implement xUnit Unit Tests for handlers and domain logic (using Moq).
- [ ] Implement Integration Tests (E2E) using `WebApplicationFactory<Program>` with a real database.

---

## Expected Outcome

What the system should enable at the completion of this change, stated in clear bullet points.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
