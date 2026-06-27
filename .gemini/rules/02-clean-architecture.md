# Rule: Clean Architecture Layering and Dependency Flow

## Metadata
- **ID**: RULE-002-CLEAN-ARCHITECTURE
- **Scope**: Solution Architecture
- **Target Layers**: All
- **Status**: Active

## Overview
The solution strictly adheres to Clean Architecture principles. Dependencies flow inward toward the core Domain layer. External details (databases, APIs, security) reside in the outer layers.

```mermaid
graph TD
    Presentation[Presentation / API] --> Application
    Presentation --> Infrastructure
    Infrastructure --> Application
    Application --> Domain
    Domain --> Shared
    Application --> Shared
    Infrastructure --> Shared
    Presentation --> Shared
```

---

## 1. Architectural Layers

### A. Domain Layer (`GreatReports.Domain`)
- **Purpose**: Core business model and rules.
- **Rules**:
  - Must have **zero dependencies** on other projects, except for `GreatReports.Shared`.
  - Must not reference external libraries (no Entity Framework, no ASP.NET Core, etc.).
  - Contains: Entities, Value Objects (if any), and Domain Validation.
  - Excludes: Repositories and external service interfaces (these are defined in Application).

### B. Application Layer (`GreatReports.Application`)
- **Purpose**: Application-specific business logic and orchestration.
- **Rules**:
  - References only `GreatReports.Domain` and `GreatReports.Shared`.
  - Contains: CQRS Command/Query models, Command/Query Handlers, DTO/Response types, and Repository Interfaces (`IUserRepository`, `IPostRepository`, `IUnitOfWork`).
  - No database-specific logic or HTTP controllers.

### C. Infrastructure Layer (`GreatReports.Infrastructure`)
- **Purpose**: External concerns and data access.
- **Rules**:
  - References `GreatReports.Application` and `GreatReports.Shared`.
  - Contains: DbContext implementations (`GreatReportsDbContext`), EF configurations, migrations, repository implementations (`UserRepository`), Identity authentication services (`IdentityService`), and external mail delivery (`SmtpEmailService`).
  - Implements the interfaces defined in the Application layer.

### D. Presentation Layer (`GreatReports.Presentation`)
- **Purpose**: Entry point (HTTP API Web API).
- **Rules**:
  - References `GreatReports.Infrastructure`, `GreatReports.Application`, and `GreatReports.Shared`.
  - Acts as the Composition Root (Dependency Injection configuration in `Configurations/DependencyInjection.cs`).
  - Contains: API controllers, HTTP request DTOs, custom exception handling middleware, and OpenAPI/Scalar API Reference settings.

### E. Shared Layer (`GreatReports.Shared`)
- **Purpose**: Common primitives shared across all projects.
- **Rules**:
  - Must remain extremely light.
  - Contains: Result patterns (`Result`, `Result<TValue>`), basic Error definitions (`Error`, `ValidationError`).

---

## 2. strict Layering Enforcement
- **Cross-Layer Leakage**: Never pass database models (Entities) directly to client responses (Presentation). Always map to DTOs/Responses in the Application layer.
- **Infrastructure Abstractions**: Always use repository interfaces (`IPostRepository`) inside Use Cases. Never inject `GreatReportsDbContext` directly into Application layer handlers.
