using System.Linq.Expressions;
using GreatReports.Infrastructure.BackgroundJobs;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;

namespace GreatReports.UnitTests.Infrastructure;

public class BackgroundJobServiceTests
{
    private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock = new();
    private readonly Mock<IRecurringJobManager> _recurringJobManagerMock = new();
    private readonly BackgroundJobService _service;

    public BackgroundJobServiceTests()
    {
        _service = new BackgroundJobService(_backgroundJobClientMock.Object, _recurringJobManagerMock.Object);
    }

    [Fact]
    public void Enqueue_ShouldCallClientCreate_WithEnqueuedState()
    {
        // Arrange
        Expression<Func<TestJob, Task>> methodCall = x => x.RunAsync();
        
        _backgroundJobClientMock
            .Setup(x => x.Create(
                It.Is<Job>(job => job.Method.Name == nameof(TestJob.RunAsync)),
                It.IsAny<EnqueuedState>()))
            .Returns("123");

        // Act
        _service.Enqueue<TestJob>(methodCall);

        // Assert
        _backgroundJobClientMock.Verify(
            x => x.Create(
                It.Is<Job>(job => job.Method.Name == nameof(TestJob.RunAsync)),
                It.IsAny<EnqueuedState>()),
            Times.Once);
    }

    [Fact]
    public void Schedule_WithDateTimeOffset_ShouldCallClientCreate_WithScheduledState()
    {
        // Arrange
        Expression<Func<TestJob, Task>> methodCall = x => x.RunAsync();
        var enqueueAt = DateTimeOffset.UtcNow.AddMinutes(10);

        _backgroundJobClientMock
            .Setup(x => x.Create(
                It.Is<Job>(job => job.Method.Name == nameof(TestJob.RunAsync)),
                It.IsAny<ScheduledState>()))
            .Returns("123");

        // Act
        _service.Schedule<TestJob>(methodCall, enqueueAt);

        // Assert
        _backgroundJobClientMock.Verify(
            x => x.Create(
                It.Is<Job>(job => job.Method.Name == nameof(TestJob.RunAsync)),
                It.IsAny<ScheduledState>()),
            Times.Once);
    }

    [Fact]
    public void Schedule_WithTimeSpan_ShouldCallClientCreate_WithScheduledState()
    {
        // Arrange
        Expression<Func<TestJob, Task>> methodCall = x => x.RunAsync();
        var delay = TimeSpan.FromMinutes(10);

        _backgroundJobClientMock
            .Setup(x => x.Create(
                It.Is<Job>(job => job.Method.Name == nameof(TestJob.RunAsync)),
                It.IsAny<ScheduledState>()))
            .Returns("123");

        // Act
        _service.Schedule<TestJob>(methodCall, delay);

        // Assert
        _backgroundJobClientMock.Verify(
            x => x.Create(
                It.Is<Job>(job => job.Method.Name == nameof(TestJob.RunAsync)),
                It.IsAny<ScheduledState>()),
            Times.Once);
    }

    [Fact]
    public void AddOrUpdate_ShouldCallRecurringJobManager_AddOrUpdate()
    {
        // Arrange
        var jobId = "test-job";
        Expression<Func<TestJob, Task>> methodCall = x => x.RunAsync();
        var cronExpression = Cron.Minutely();

        // Act
        _service.AddOrUpdate<TestJob>(jobId, methodCall, cronExpression);

        // Assert
        _recurringJobManagerMock.Verify(x => x.AddOrUpdate(
            jobId,
            It.Is<Job>(job => job.Method.Name == nameof(TestJob.RunAsync)),
            cronExpression,
            It.IsAny<RecurringJobOptions>()),
            Times.Once);
    }

    public class TestJob
    {
        public Task RunAsync() => Task.CompletedTask;
    }
}
