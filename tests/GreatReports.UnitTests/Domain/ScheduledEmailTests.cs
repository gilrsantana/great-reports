using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using System;
using Xunit;

namespace GreatReports.UnitTests.Domain;

public class ScheduledEmailTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenGroupIdIsEmpty()
    {
        // Act
        var result = ScheduledEmail.Create(Guid.Empty, "Relatório Diário", "0 0 * * *", ReportFrequency.Daily);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ScheduledEmail.InvalidGroup", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Act
        var result = ScheduledEmail.Create(Guid.NewGuid(), "", "0 0 * * *", ReportFrequency.Daily);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ScheduledEmail.InvalidName", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenCronExpressionIsEmpty()
    {
        // Act
        var result = ScheduledEmail.Create(Guid.NewGuid(), "Relatório Diário", "", ReportFrequency.Daily);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ScheduledEmail.InvalidCron", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenFrequencyIsSpecificDayAndDayIsNull()
    {
        // Act
        var result = ScheduledEmail.Create(
            Guid.NewGuid(),
            "Relatório Mensal",
            "0 0 5 * *",
            ReportFrequency.SpecificDay,
            null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ScheduledEmail.InvalidSpecificDay", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenFrequencyIsSpecificDayAndDayIsLessThanOne()
    {
        // Act
        var result = ScheduledEmail.Create(
            Guid.NewGuid(),
            "Relatório Mensal",
            "0 0 5 * *",
            ReportFrequency.SpecificDay,
            0);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ScheduledEmail.InvalidSpecificDay", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenFrequencyIsSpecificDayAndDayIsGreaterThanThirtyOne()
    {
        // Act
        var result = ScheduledEmail.Create(
            Guid.NewGuid(),
            "Relatório Mensal",
            "0 0 5 * *",
            ReportFrequency.SpecificDay,
            32);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ScheduledEmail.InvalidSpecificDay", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var name = "Relatório Semanal";
        var cron = "0 0 * * 1";
        var frequency = ReportFrequency.Weekly;

        // Act
        var result = ScheduledEmail.Create(groupId, name, cron, frequency);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(groupId, result.Value.GroupId);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(cron, result.Value.CronExpression);
        Assert.Equal(frequency, result.Value.Frequency);
        Assert.Null(result.Value.SpecificDayOfMonth);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenFrequencyIsSpecificDayAndDayIsValid()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var name = "Relatório Mensal";
        var cron = "0 0 10 * *";
        var frequency = ReportFrequency.SpecificDay;
        var specificDay = 10;

        // Act
        var result = ScheduledEmail.Create(groupId, name, cron, frequency, specificDay);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(groupId, result.Value.GroupId);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(cron, result.Value.CronExpression);
        Assert.Equal(frequency, result.Value.Frequency);
        Assert.Equal(specificDay, result.Value.SpecificDayOfMonth);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (ScheduledEmail)Activator.CreateInstance(typeof(ScheduledEmail), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
