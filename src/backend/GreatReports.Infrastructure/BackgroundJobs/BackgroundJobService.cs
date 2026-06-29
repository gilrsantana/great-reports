using System.Linq.Expressions;
using GreatReports.Application.Common.Interfaces;
using Hangfire;

namespace GreatReports.Infrastructure.BackgroundJobs;

public class BackgroundJobService(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager) : IBackgroundJobService
{
    public void Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        backgroundJobClient.Enqueue<T>(methodCall);
    }

    public void Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
    {
        backgroundJobClient.Schedule<T>(methodCall, enqueueAt);
    }

    public void Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        backgroundJobClient.Schedule<T>(methodCall, delay);
    }

    public void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression)
    {
        recurringJobManager.AddOrUpdate<T>(recurringJobId, methodCall, cronExpression);
    }
}
