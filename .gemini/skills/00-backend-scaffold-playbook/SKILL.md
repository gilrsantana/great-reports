---
name: backend-scaffold-playbook
description: Master playbook to guide the chronological creation of a new backend feature (domain, use cases, DB mapping, DI, controller, and tests) by linking individual skills.
---

# Playbook: Scaffolding a New Backend Feature Slice

Use this playbook when you need to add a new entity or feature to the application. You must follow the steps below sequentially, utilizing the individual referenced skills.

---

## The Scaffolding Sequence Checklist

### 🏁 Phase 1: Core Domain Modeling
- [ ] Create the domain entity inheriting from `BaseEntity` (which provides `Id`, `CreatedAt`, `UpdatedAt`, `Active`, and `UnActivateDate`) with proper property encapsulation.
  - 👉 *Refer to skill:* [01-create-domain-entity](../01-create-domain-entity/SKILL.md)

### ⚙️ Phase 2: Application Use Cases
- [ ] Implement the Commands, Queries, and Response records.
- [ ] Write the Handlers implementing `ICommandHandler` or `IQueryHandler`.
- [ ] Implement manual mappings inside the handlers.
  - 👉 *Refer to skill:* [02-create-use-case](../02-create-use-case/SKILL.md)

### 🗄️ Phase 3: Infrastructure and Database Mapping
- [ ] Add the database Fluent API configuration under `Persistence/Configurations/`.
- [ ] Create the repository inheriting from `BaseEntityRepository<TEntity>`.
  - 👉 *Refer to skill:* [04-create-infrastructure-mapping](../04-create-infrastructure-mapping/SKILL.md)

### 🚀 Phase 4: Database Migrations
- [ ] Add and apply EF Core migrations to update your local relational database schema.
  - 👉 *Refer to skill:* [05-database-migrations](../05-database-migrations/SKILL.md)

### 🔌 Phase 5: Dependency Injection Wiring
- [ ] Register the CQRS Use Case handlers in `Application/Extensions/DependencyInjection.cs`.
- [ ] Register the Repository interface mapping in `Infrastructure/Extensions/DependencyInjection.cs`.
  - 👉 *Refer to skill:* [06-configure-dependency-injection](../06-configure-dependency-injection/SKILL.md)

### 🌐 Phase 6: Exposing the API
- [ ] Create the Controller inheriting from `ApiControllerBase`.
- [ ] Inject Use Case handlers and expose actions using `HandleResult`.
- [ ] Annotate actions with appropriate `[ProducesResponseType]` OpenAPI attributes.
  - 👉 *Refer to skill:* [07-create-api-controller](../07-create-api-controller/SKILL.md)

### 🧪 Phase 7: Verification and Testing
- [ ] Write unit tests for domain logic and application handlers using `Moq` for mocks.
  - 👉 *Refer to skill:* [08-create-unit-test](../08-create-unit-test/SKILL.md)
- [ ] Run `dotnet build` to ensure the entire solution compiles with zero errors.

### 📑 Phase 8: Documentation (OKF Compliance)
- [ ] Document all new domain entities, database tables, and API endpoints.
- [ ] Ensure all generated/updated markdown files comply with OKF v0.1 guidelines, including YAML frontmatter.
- [ ] Update the folder `index.md` files and append a log entry to the central `log.md` file.
  - 👉 *Refer to rule:* [13-open-knowledge-format](../../rules/13-open-knowledge-format.md)
  - 👉 *Refer to skill:* [12-manage-okf-documentation](../12-manage-okf-documentation/SKILL.md)
