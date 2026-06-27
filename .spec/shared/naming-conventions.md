# Naming Conventions

Global conventions for file and directory names. These can be referenced by any change specification.

## General Rules

- **Frontend & Folder Structures**: Folder names and typescript file names must be in `kebab-case`, using lowercase only (e.g., `partner-list`, `auth-service`).
- **Backend (.NET)**: C# class names, namespaces, and C# source file names must use `PascalCase` (e.g., `ProviderCompany.cs`, `CompileDailyActivityJob.cs`).
- Naming must indicate **responsibility**, not just technical implementation details.

- **Language Boundaries (RULE-014)**:
  - Technical declarations, classes, parameters, database schemas, and configuration fields use **US-English**.
  - User-facing text, page layouts, components, charts, templates, email bodies, and error description messages use **BR-Portuguese**.

---

## C# / .NET 10 Naming Conventions

All backend code follows standard Microsoft coding conventions, plus specialized Clean Architecture, EF Core (`MySql.EntityFrameworkCore`), and Hangfire suffixes.

| Suffix / Pattern | Path / Context | Purpose | Example |
| :--- | :--- | :--- | :--- |
| `{Entity}.cs` | `src/backend/GreatReports.Domain/Entities/` | Domain models inheriting from `BaseEntity` | `ProviderCompany.cs` |
| `{Entity}Configuration.cs` | `src/backend/GreatReports.Infrastructure/Persistence/Configurations/` | Fluent API mapping config for EF Core using MySQL | `ProviderCompanyConfiguration.cs` |
| `I{Entity}Repository.cs` | `src/backend/GreatReports.Application/Common/Interfaces/` | Repository interface contracts | `IProviderCompanyRepository.cs` |
| `{Entity}Repository.cs` | `src/backend/GreatReports.Infrastructure/Persistence/Repositories/` | Repo implementations inheriting from `BaseEntityRepository<T>` | `ProviderCompanyRepository.cs` |
| `{UseCase}Command.cs` | `src/backend/GreatReports.Application/UseCases/{Feature}/Commands/` | CQRS Command record | `CreateProviderCompanyCommand.cs` |
| `{UseCase}CommandHandler.cs`| `src/backend/GreatReports.Application/UseCases/{Feature}/CommandHandlers/` | CQRS Command Handler class | `CreateProviderCompanyCommandHandler.cs` |
| `{UseCase}Query.cs` | `src/backend/GreatReports.Application/UseCases/{Feature}/Queries/` | CQRS Query record | `GetProviderCompanyByIdQuery.cs` |
| `{UseCase}QueryHandler.cs`| `src/backend/GreatReports.Application/UseCases/{Feature}/QueryHandlers/` | CQRS Query Handler class | `GetProviderCompanyByIdQueryHandler.cs` |
| `{Entity}Controller.cs` | `src/backend/GreatReports.Presentation/Controllers/` | Web API controller inheriting from `ApiControllerBase` | `ProviderCompaniesController.cs` |
| `{Job}Job.cs` | `src/backend/GreatReports.Application/ApplicationJobs/` | Background job executing Hangfire routines | `CompileDailyActivityJob.cs` |
| `{Class}Tests.cs` | `tests/GreatReports.UnitTests/` or `IntegrationTests/` | Test classes | `CompileDailyActivityJobTests.cs` |

---

## TypeScript / Angular 22 Naming Conventions

All frontend source files (except system files like `package.json`) use `kebab-case` filenames and `PascalCase` class names.

| Suffix / Pattern | Path / Context | Purpose | Example |
| :--- | :--- | :--- | :--- |
| `*.models.ts` | `src/frontend/src/app/core/models/` | Type interfaces representing DTOs | `provider-company.models.ts` |
| `*.service.ts` | `src/frontend/src/app/core/services/` | Singleton HTTP client service wrapping | `provider-company.service.ts` |
| `*.component.ts` | `src/frontend/src/app/features/{feature}/` | Angular standalone component logic | `group-list.component.ts` |
| `*.component.html` | `src/frontend/src/app/features/{feature}/` | Angular component template | `group-list.component.html` |
| `*.component.css` | `src/frontend/src/app/features/{feature}/` | Component specific styles (Tailwind theme variables) | `group-list.component.css` |
| `*.routes.ts` | `src/frontend/src/app/features/{feature}/` | Feature sub-routes for lazy-loading | `group.routes.ts` |

---

## Controlled Exceptions

Names forced by external tools, frameworks, or package definitions maintain their native formats (e.g., `Program.cs`, `appsettings.json`, `package.json`, `tsconfig.json`, `spec.md`, `README.md`).
