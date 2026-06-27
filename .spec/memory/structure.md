# Repository Structure

## Target Directory Layout

```text
.spec/
  changes/              # Change-specific specifications (incremental specs)
  memory/               # Global project context and product definitions
  shared/               # Reusable rules and conventions between specs
  templates/            # Templates for creating new change specs
docs/                   # System ground truth documentation (Google OKF v0.1 format)
  diagrams/
    activity/           # Workflow and background activity diagrams (Mermaid stateDiagram-v2)
    c4/                 # Level 1/2 System Context & Container diagrams (Mermaid flowchart)
    class/              # Domain entities UML class diagrams (Mermaid classDiagram)
    use-case/           # Role-based use case diagrams (Mermaid flowchart LR)
  index.md              # Root index listing documentation sections
  log.md                # Chronological update log of system documentation
  README.md             # Main entry point, architecture overview
src/
  backend/                      # Backend Clean Architecture solution files
    GreatReports.slnx           # Backend Solution File
    GreatReports.Domain/       # Domain entities, value objects, domain validations
    GreatReports.Application/  # Use cases, CQRS handlers, interfaces, DTOs, Hangfire Jobs
    GreatReports.Infrastructure/ # DbContext, MySql.EntityFrameworkCore mappings, Repositories, Resend client
    GreatReports.Presentation/ # Controllers, composition root, OpenAPI/Scalar, Hangfire Dashboard auth
    GreatReports.Shared/       # Shared primitives (Result, Error, ValidationError)
  frontend/                     # Angular 22 standalone frontend application styled with Tailwind v4
tests/
  GreatReports.UnitTests/      # Unit tests mirroring source layers (using xUnit and Moq)
  GreatReports.IntegrationTests/ # E2E Integration tests with real MySQL database connectivity
```

## Layer Responsibilities

- **`.spec/`**: Contains specifications written before the code is implemented to drive incremental development.
- **`docs/`**: Holds the formal architectural models, diagram files, and schema mappings under Google OKF v0.1 format.
- **`src/backend/GreatReports.Domain`**: Core business invariants. Entities (such as `ProviderCompany`, `ClientCompany`, `ClientContact`, `Project`, `User`, `Group`, `ScheduledEmail`) inherit from `BaseEntity`. Zero dependencies on external layers or frameworks.
- **`src/backend/GreatReports.Application`**: Handlers for CQRS Commands and Queries. Interfaces for repositories and services. Positional records for DTOs. Location of Hangfire job definitions (e.g. `CompileDailyActivityJob`).
- **`src/backend/GreatReports.Infrastructure`**: Connection configurations, `MySql.EntityFrameworkCore` Fluent API configurations (`Persistence/Configurations/`), repository implementations inheriting from `BaseEntityRepository<TEntity>`, Resend client adapter, and Hangfire MySQL backend configuration.
- **`src/backend/GreatReports.Presentation`**: REST HTTP Controllers inheriting from `ApiControllerBase`. Hangfire Dashboard integration with custom authentication/authorization filters. Contains `Configurations/DependencyInjection.cs` as composition root.
- **`src/backend/GreatReports.Shared`**: Light project providing the Result pattern primitives (`Result`, `Result<TValue>`, `Error`, `ValidationError`).
- **`src/frontend/`**: Angular 22 standalone UI structured by feature slices (e.g., admin administration, group leader group routing, partner activities dashboard, client company dashboards with charts).
- **`tests/`**: Validates the codebase. Unit tests use `Moq` for mocks, and integration tests launch the system via `WebApplicationFactory<Program>` pointing to a test MySQL instance.
