using System.Linq.Expressions;
using GreatReports.Application.Common.Interfaces;
using Hangfire;

namespace GreatReports.Infrastructure.BackgroundJobs;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;

    public BackgroundJobService(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }

    public void Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        _backgroundJobClient.Enqueue<T>(methodCall);
    }

    public void Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
    {
        _backgroundJobClient.Schedule<T>(methodCall, enqueueAt);
    }

    public void Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        _backgroundJobClient.Schedule<T>(methodCall, delay);
    }

    public void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression)
    {
        _recurringJobManager.AddOrUpdate<T>(recurringJobId, methodCall, cronExpression);
    }
}
