# 004-configure-background-job

## Objective

Configure a decoupled background job processing environment using Hangfire with MySQL storage, and implement a wrapper service to schedule and execute asynchronous jobs safely.

## Technical Context

This change integrates Hangfire background job configuration into the application, separating the application layer from concrete Hangfire libraries using a wrapper interface. The database storage is powered by MySQL, and the dashboard is secured utilizing JWT-based authentication so only accounts with the `Maintainer` role can access it.

> [!IMPORTANT]
> - Refer to Skill `14` (Hangfire Background Jobs) for implementation details.
> - Refer to RULE-012 (Background Processing and Orchestration) for class naming, placement, and lifecycle rules.
> - Keep worker count constrained (e.g., restricted worker count of 2) in shared/low-resource database environments to avoid database connection exhaustion.
> - Follow localization rules (RULE-014) for any error/outcome messages (use Portuguese-BR).

## Project References

- [Product Definition](../../../memory/product.md)
- [Global Technical Context](../../../memory/technical-context.md)
- [Repository Structure](../../../memory/structure.md)

## Shared References

- [How to Run](../../../shared/how-to-run.md)
- [Naming Conventions](../../../shared/naming-conventions.md)

## Local Observations

- The Hangfire dashboard must validate JWT tokens extracted from a cookie named `HangfireToken` or from a query parameter `token`.
- Only accounts with the `Maintainer` role have access to the Hangfire Dashboard.

---

## Tasks

### Tasks - Application Layer (`GreatReports.Application`)
- [x] Define interface `IBackgroundJobService` in `src/backend/GreatReports.Application/Common/Interfaces/IBackgroundJobService.cs` under `GreatReports.Application.Common.Interfaces` namespace with:
  - `void Enqueue<T>(Expression<Func<T, Task>> methodCall);`
  - `void Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt);`
  - `void Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);`
  - `void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression);`
>  ✅ 2026-06-28 19:03 - Created mockable, generic IBackgroundJobService interface.

### Tasks - Infrastructure Layer (`GreatReports.Infrastructure`)
- [x] Implement wrapper class `BackgroundJobService` implementing `IBackgroundJobService` using Hangfire in `src/backend/GreatReports.Infrastructure/BackgroundJobs/BackgroundJobService.cs` under `GreatReports.Infrastructure.BackgroundJobs` namespace.
>  ✅ 2026-06-28 19:03 - Implemented BackgroundJobService using Hangfire APIs.
- [x] Register `IBackgroundJobService` as a scoped dependency in `src/backend/GreatReports.Infrastructure/Extensions/DependencyInjection.cs`.
>  ✅ 2026-06-28 19:04 - Registered the background job wrapper in the DI extension.
- [x] Configure `SetHangfire` in `DependencyInjection.cs` to apply worker configuration limits (e.g., limit worker count to 2 to prevent connection exhaustion).
>  ✅ 2026-06-28 19:04 - Constrained worker count to 2 inside SetHangfire database setup.

### Tasks - Presentation Layer (`GreatReports.Presentation`)
- [x] Implement custom authorization filter `HangfireDashboardAuthorizationFilter` implementing `IDashboardAuthorizationFilter` under `src/backend/GreatReports.Presentation/Filters/HangfireDashboardAuthorizationFilter.cs` to authenticate and authorize users in the `Maintainer` role using JWT.
>  ✅ 2026-06-28 19:04 - Created custom filter reading JWT from cookie or query param.
- [x] Register `JwtSettings` options and extract secrets, issuer, audience to configure `HangfireDashboardAuthorizationFilter` in `UsePresentationPipeline` middleware registration inside `src/backend/GreatReports.Presentation/Extensions/DependencyInjection.cs`.
>  ✅ 2026-06-28 19:04 - Resolved JwtSettings inside composition root.
- [x] Map Hangfire Dashboard to `/hangfire` route with custom authorization filter options and `IgnoreAntiforgeryToken = true`.
>  ✅ 2026-06-28 19:04 - Mapped and secured dashboard route using the filter.

### Tasks - Verification & Testing
- [x] Implement unit tests in `tests/GreatReports.UnitTests/Infrastructure/BackgroundJobServiceTests.cs` to verify:
  - The `BackgroundJobService` delegates calls correctly to Hangfire's standard client (`IBackgroundJobClient`) and recurring manager (`IRecurringJobManager`).
>  ✅ 2026-06-28 19:04 - Added unit tests mocking IBackgroundJobClient and IRecurringJobManager.
- [x] Implement unit tests in `tests/GreatReports.UnitTests/Presentation/HangfireDashboardAuthorizationFilterTests.cs` to verify token validation and role checks (granting access to `Maintainer` and denying others).
>  ✅ 2026-06-28 19:05 - Added unit tests validating JWT signatures and role claims with AspNetCoreDashboardContext.
- [x] Verify that the whole project builds and runs without errors.
>  ✅ 2026-06-28 19:06 - Verified all 134 unit tests pass successfully.

---

## Expected Outcome

- A fully functional, decoupled background job processing engine powered by Hangfire.
- Secure Hangfire Dashboard mapped to `/hangfire` that is restricted to `Maintainer` accounts via JWT.
- Clean application code utilizing `IBackgroundJobService` without dependency on Hangfire NuGet packages.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../../shared/how-to-run.md).
