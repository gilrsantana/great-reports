# 001-create-project

## Objective

Initialize the Great Reports workspace by scaffolding the .NET 10 Clean Architecture backend solution and the Angular 22 standalone frontend application. 

This scaffolding setup will integrate MySQL (using the official `MySql.EntityFrameworkCore` package), configure the Hangfire background processor with MySQL storage, set up ASP.NET Core Identity (establishing the separated `Account` and `Role` concept), and apply Tailwind CSS v4 styling, ensuring all components adhere to the project's rules and skills.

## Technical Context

This change sets up the foundation. It establishes the multi-project backend layers, shared Result/Error primitives, custom CQRS base structures, EF Core MySQL DbContext, Hangfire orchestration pipeline, Identity authentication config, and the frontend Angular shell.

> [!IMPORTANT]
> The scaffolding process must read and strictly implement:
> - `RULE-001` (Formatting & Allman braces)
> - `RULE-002` (Clean Architecture boundaries)
> - `RULE-003` (Shared Result and Error primitives)
> - `RULE-005` (CQRS marker interfaces)
> - `RULE-006` / `RULE-007` (MySQL configuration using `MySql.EntityFrameworkCore`)
> - `RULE-008` (ApiControllerBase & Scalar UI)
> - `RULE-009` (Modular Dependency Injection registration)
> - `RULE-011` / Skill `09` (Tailwind CSS v4 & Standalone Angular 22 structure)
> - `RULE-012` (Background processing setup)
> - Skill `08` (Setting up Identity and Auth - configuring `Account`, `Role`, `JwtSettings` options)

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

---

## Tasks

### Tasks - Solution & Project Scaffolding
- [x] Create the .NET solution file `great-reports.sln` at the `src/backend/` directory.
>  ✅ 2026-06-27 07:26 - Scaffolded `great-reports.slnx` (the new XML solution format standard in .NET 10).
- [x] Create project layers inside `src/backend/`:
  - `src/backend/great-reports.Shared/great-reports.Shared.csproj` (Class Library, target net10.0)
  - `src/backend/great-reports.Domain/great-reports.Domain.csproj` (Class Library, target net10.0)
  - `src/backend/great-reports.Application/great-reports.Application.csproj` (Class Library, target net10.0)
  - `src/backend/great-reports.Infrastructure/great-reports.Infrastructure.csproj` (Class Library, target net10.0)
  - `src/backend/great-reports.Presentation/great-reports.Presentation.csproj` (Web API, target net10.0)
>  ✅ 2026-06-27 07:26 - Created the 5 layers as net10.0 class libraries and Web API project using `dotnet new`.
- [x] Add all projects to the solution `great-reports.sln` under `src/backend/`.
>  ✅ 2026-06-27 07:26 - Added all project layers to `great-reports.slnx` using `dotnet sln`.
- [x] Establish Clean Architecture references (following `RULE-002`):
  - `Domain` references `Shared`.
  - `Application` references `Domain`, `Shared`.
  - `Infrastructure` references `Application`, `Shared`.
  - `Presentation` references `Infrastructure`, `Application`, `Shared`.
>  ✅ 2026-06-27 07:26 - References established using `dotnet add reference`.

### Tasks - Shared Primitives & Domain Base (Applying RULE-001, RULE-003, RULE-004)
- [x] Create `Error` record and `ValidationError` record in `src/backend/great-reports.Shared`.
>  ✅ 2026-06-27 07:26 - Created `Error.cs` and `ValidationError.cs` records with file-scoped namespaces.
- [x] Create `Result` and `Result<TValue>` helper classes in `src/backend/great-reports.Shared`.
>  ✅ 2026-06-27 07:26 - Created `Result.cs` featuring implicit value conversion and type safety.
- [x] Create `BaseEntity` abstract class in `src/backend/great-reports.Domain/Entities/` featuring UUID v7 generation and Timestamp variables.
>  ✅ 2026-06-27 07:26 - Implemented `BaseEntity.cs` using native `Guid.CreateVersion7()`.
- [x] Ensure all C# files use file-scoped namespaces and Allman brace formatting.
>  ✅ 2026-06-27 07:26 - Verified all files strictly follow Allman braces and file-scoped namespace format.

### Tasks - Hand-Rolled CQRS Scaffolding (Applying RULE-005)
- [x] Create CQRS marker interfaces (`ICommand`, `ICommand<TResponse>`, `IQuery<TResponse>`) in `src/backend/great-reports.Application/Common/CQRS/`.
>  ✅ 2026-06-27 07:26 - Defined `ICommand`, `ICommand<TResponse>`, and `IQuery<TResponse>` marker interfaces.
- [x] Create CQRS handler contracts (`ICommandHandler<TCommand>`, `ICommandHandler<TCommand, TResponse>`, `IQueryHandler<TQuery, TResponse>`) in `src/backend/great-reports.Application/Common/CQRS/`.
>  ✅ 2026-06-27 07:26 - Created handler interfaces `ICommandHandler` and `IQueryHandler`.

### Tasks - Infrastructure MySQL & Identity Setup (Applying RULE-006, RULE-007, Skill 08)
- [x] Add package dependency `MySql.EntityFrameworkCore` to `src/backend/great-reports.Infrastructure/great-reports.Infrastructure.csproj`.
>  ✅ 2026-06-27 07:27 - Added the MySQL EF Core dependency via NuGet.
- [x] Add package dependency `Microsoft.AspNetCore.Identity.EntityFrameworkCore` and `System.IdentityModel.Tokens.Jwt` to `src/backend/great-reports.Infrastructure/great-reports.Infrastructure.csproj`.
>  ✅ 2026-06-27 07:27 - Packages successfully added to the Infrastructure project.
- [x] Add package dependency `Microsoft.AspNetCore.Authentication.JwtBearer` to `src/backend/great-reports.Presentation/great-reports.Presentation.csproj`.
>  ✅ 2026-06-27 07:27 - Package added to Presentation layer.
- [x] Define the custom Identity entities `Account` and `Role` inside `src/backend/great-reports.Infrastructure/Identity/`.
>  ✅ 2026-06-27 07:27 - Defined `Account` inheriting from `IdentityUser<Guid>` and `Role` from `IdentityRole<Guid>`.
- [x] Configure `great-reportsDbContext.cs` inheriting from `IdentityDbContext<Account, Role, Guid>` in `src/backend/great-reports.Infrastructure/Persistence/`.
>  ✅ 2026-06-27 07:28 - Created `GreatReportsDbContext.cs` inheriting from `IdentityDbContext`.
- [x] Apply separate `AccountConfiguration` and `RoleConfiguration` mapping them to `"Accounts"` and `"Roles"` tables (following Skill 08).
>  ✅ 2026-06-27 07:28 - Added explicit mappings to the target tables in configurations folder.
- [x] Implement `DatabaseOptions` and `JwtSettings` binding classes in `src/backend/great-reports.Infrastructure/Configurations/`.
>  ✅ 2026-06-27 07:28 - Created option binding classes mapping DB connection string and JWT options.
- [x] Configure `BaseEntityRepository<TEntity>` implementing `IUnitOfWork` inside `src/backend/great-reports.Infrastructure/Persistence/Repositories/`.
>  ✅ 2026-06-27 07:28 - Created repository base implementing `IUnitOfWork`.

### Tasks - Hangfire & Presentation Setup (Applying RULE-008, RULE-009, RULE-012)
- [x] Add package dependency `Hangfire.AspNetCore` and `Hangfire.MySql` to `src/backend/great-reports.Infrastructure/great-reports.Infrastructure.csproj` and `src/backend/great-reports.Presentation/great-reports.Presentation.csproj`.
>  ✅ 2026-06-27 07:27 - Added `Hangfire.AspNetCore` and `Hangfire.MySqlStorage` packages.
- [x] Bind MySQL, Hangfire, Identity, and JWT Bearer settings inside modular dependency registrations.
>  ✅ 2026-06-27 07:28 - Registered services in `Configurations/DependencyInjection.cs`.
- [x] Create `ApiControllerBase` in `src/backend/great-reports.Presentation/Controllers/` with standardized `HandleResult` mapping and RFC 7807 `ProblemDetails` output formatting.
>  ✅ 2026-06-27 07:28 - Implemented `ApiControllerBase.cs` mapping `Result` outcomes to RFC 7807 problem json.
- [x] Create `CustomExceptionHandlingMiddleware` for unhandled 500 runtime logs.
>  ✅ 2026-06-27 07:28 - Created middleware to catch global exceptions and return ProblemDetails.
- [x] Configure native OpenAPI services and wire up **Scalar API Reference** UI.
>  ✅ 2026-06-27 07:28 - Setup `AddOpenApi()` and `MapScalarApiReference()` mapping.
- [x] Implement composition root extension methods in `src/backend/great-reports.Presentation/Configurations/DependencyInjection.cs` to modularly chain layer-specific configurations and pipeline middlewares.
>  ✅ 2026-06-27 07:28 - Composition root extension methods implemented.
- [x] Clean up `src/backend/great-reports.Presentation/Program.cs` to only call the composition root.
>  ✅ 2026-06-27 07:28 - Program.cs cleaned up to execute composition root methods.

### Tasks - Angular 22 & Tailwind Scaffolding (Applying RULE-011, Skill 09)
- [x] Initialize the Angular 22 application inside the `src/frontend/` folder.
>  ✅ 2026-06-27 07:29 - Scaffolded standalone Angular 22 app with routing.
- [x] Configure Tailwind CSS v4 in `src/frontend/src/styles.css` using theme variables (`--color-bg-primary`, `--color-bg-secondary`, `--color-accent-brand`, `--transition-smooth`).
>  ✅ 2026-06-27 07:30 - Added `@import "tailwindcss";` and `@theme` block config to `styles.css`.
- [x] Setup base routes in `src/frontend/src/app/app.routes.ts`.
>  ✅ 2026-06-27 07:30 - Wired routes for LoginComponent and DashboardComponent.

### Tasks - Verification & Testing (Applying RULE-010)
- [x] Create xUnit project `tests/great-reports.UnitTests/` and add to solution.
>  ✅ 2026-06-27 07:30 - Created unit tests project targeting .NET 10.0.
- [x] Create xUnit project `tests/great-reports.IntegrationTests/` and add to solution.
>  ✅ 2026-06-27 07:30 - Created integration tests project targeting .NET 10.0 (execution disabled as per user instruction).
- [x] Run `dotnet build` to ensure the entire solution compiles without errors.
>  ✅ 2026-06-27 07:31 - Compiled successfully with 0 errors and warning-free codebase.

---

## Expected Outcome

- A clean C#/.NET 10 solution structured by clean layers compiling with zero warnings.
- Integrated MySQL client access through `MySql.EntityFrameworkCore`.
- ASP.NET Core Identity authentication configuration active on endpoints.
- An active Hangfire dashboard route and MySQL background storage connection.
- An initialized Angular 22 standalone frontend project compiled with Tailwind CSS v4.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
