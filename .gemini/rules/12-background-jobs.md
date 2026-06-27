# Rule: Background Processing and Orchestration

## Metadata
- **ID**: RULE-012-BACKGROUND-PROCESSING
- **Scope**: Application & Infrastructure Layers
- **Target Types**: Job Definitions, Job Schedulers
- **Status**: Active

## Overview
This rule outlines the patterns for background job scheduling, execution, and organization. It ensures background processing is clean, transactional, and runs safely without blocking HTTP request pipelines.

---

## 1. Job Organization & Namespace
All background job logic must be placed inside the Application project:
- **Location**: `src/GreatReports.Application/ApplicationJobs/`
- **Namespace**: `GreatReports.Application.ApplicationJobs`
- **Class Naming**: Use the suffix `Job` (e.g., `EmailSyncJob.cs`, `CleanLogsJob.cs`).

---

## 2. Job Class Patterns & Constructor Injection
- Jobs are standard classes. They must not inherit from base classes unless explicitly required by a framework extension.
- **Dependencies**: Inject repositories, services, and `IUnitOfWork` using the standard constructor injection pattern. Do not reference `GreatReportsDbContext` directly.
- **Method Execution**: The executing method must be asynchronous, return `Task`, and accept a `CancellationToken`.

### Example Job Definition:
```csharp
using GreatReports.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GreatReports.Application.ApplicationJobs;

public class EmailSyncJob
{
    private readonly IEmailAuditLogRepository _auditLogRepository;
    private readonly ILogger<EmailSyncJob> _logger;

    public EmailSyncJob(
        IEmailAuditLogRepository auditLogRepository,
        ILogger<EmailSyncJob> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting email sync process...");
        
        // Execute operational checks...
        var count = await _auditLogRepository.GetPendingSyncCountAsync(cancellationToken);
        _logger.LogInformation("Processed {Count} pending emails", count);
    }
}
```

---

## 3. Job Registration & Scheduling
- **Composition Root**: Register the job class as `Transient` or `Scoped` inside the Presentation project's `Configurations/DependencyInjection.cs` file.
- **Scheduling Invocation**:
  - For **One-Time/Enqueue** tasks: Use background client queues inside Use Case handlers.
  - For **Recurring** tasks: Use job manager schedules during system startup in the Presentation composition root.
  - Standard recurring job configuration must occur inside `Configurations/DependencyInjection.cs` during application pipeline initialization:
    ```csharp
    public static void ConfigureRecurringJobs(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        
        manager.AddOrUpdate<EmailSyncJob>(
            "email-synchronization",
            job => job.ExecuteAsync(CancellationToken.None),
            Cron.Minutely());
    }
    ```

---

## 4. Operational Invariant Rules
1. **Idempotency**: All jobs must be designed to be idempotent. In the event of network timeouts, scheduling systems may retry the execution.
2. **Cancellation Handling**: Always propagate `CancellationToken` to downstream repository queries to ensure resources are released immediately if a job is cancelled/killed.
3. **Transaction Scope**: Explicitly call `SaveChangesAsync()` inside the job methods using `IUnitOfWork` to save state changes, rather than assuming automatic persistence.
