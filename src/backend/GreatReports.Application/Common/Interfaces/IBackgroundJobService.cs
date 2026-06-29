using System.Linq.Expressions;

namespace GreatReports.Application.Common.Interfaces;

public interface IBackgroundJobService
{
    /// <summary>
    /// Enqueues a job for immediate execution (Fire-and-Forget).
    /// </summary>
    void Enqueue<T>(Expression<Func<T, Task>> methodCall);

    /// <summary>
    /// Schedules a job to run at a specific date and time (Delayed).
    /// </summary>
    void Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt);

    /// <summary>
    /// Schedules a job to run after a specific delay (Delayed).
    /// </summary>
    void Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);

    /// <summary>
    /// Registers or updates a recurring job (Recurring).
    /// </summary>
    void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression);
}
