using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using System;
using Xunit;

namespace GreatReports.UnitTests.Domain;

public class DailyActivityTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenPartnerIdIsEmpty()
    {
        // Act
        var result = DailyActivity.Create(
            Guid.Empty,
            "Título",
            "Tema",
            "Conteúdo detalhado",
            DateTime.UtcNow,
            ActivityStatus.Done,
            false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("DailyActivity.InvalidPartner", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Act
        var result = DailyActivity.Create(
            Guid.NewGuid(),
            "",
            "Tema",
            "Conteúdo detalhado",
            DateTime.UtcNow,
            ActivityStatus.Done,
            false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("DailyActivity.InvalidTitle", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenThemeIsEmpty()
    {
        // Act
        var result = DailyActivity.Create(
            Guid.NewGuid(),
            "Título",
            "",
            "Conteúdo detalhado",
            DateTime.UtcNow,
            ActivityStatus.Done,
            false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("DailyActivity.InvalidTheme", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenContentIsEmpty()
    {
        // Act
        var result = DailyActivity.Create(
            Guid.NewGuid(),
            "Título",
            "Tema",
            "",
            DateTime.UtcNow,
            ActivityStatus.Done,
            false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("DailyActivity.InvalidContent", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenReferenceDateIsDefault()
    {
        // Act
        var result = DailyActivity.Create(
            Guid.NewGuid(),
            "Título",
            "Tema",
            "Conteúdo detalhado",
            default,
            ActivityStatus.Done,
            false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("DailyActivity.InvalidReferenceDate", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var title = "Atividade 1";
        var theme = "Tema 1";
        var content = "Conteúdo detalhado da atividade";
        var referenceDate = DateTime.UtcNow;
        var status = ActivityStatus.Done;

        // Act
        var result = DailyActivity.Create(partnerId, title, theme, content, referenceDate, status, false);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(partnerId, result.Value.PartnerId);
        Assert.Equal(title, result.Value.Title);
        Assert.Equal(theme, result.Value.Theme);
        Assert.Equal(content, result.Value.Content);
        Assert.Equal(referenceDate, result.Value.ReferenceDate);
        Assert.Equal(status, result.Value.Status);
        Assert.False(result.Value.IsBlocked);
        Assert.False(result.Value.IsPublished);
        Assert.Null(result.Value.SummarizedContent);
        Assert.Null(result.Value.ProcessedAt);
    }

    [Fact]
    public void Publish_ShouldSetIsPublishedToTrue_WhenCalled()
    {
        // Arrange
        var activity = DailyActivity.Create(
            Guid.NewGuid(),
            "Título",
            "Tema",
            "Conteúdo detalhado",
            DateTime.UtcNow,
            ActivityStatus.Done,
            false).Value;

        // Act
        activity.Publish();

        // Assert
        Assert.True(activity.IsPublished);
    }

    [Fact]
    public void SetProcessed_ShouldSetSummarizedContentAndProcessedAt_WhenCalled()
    {
        // Arrange
        var activity = DailyActivity.Create(
            Guid.NewGuid(),
            "Título",
            "Tema",
            "Conteúdo detalhado",
            DateTime.UtcNow,
            ActivityStatus.Done,
            false).Value;
        var summary = "Resumo da atividade";

        // Act
        activity.SetProcessed(summary);

        // Assert
        Assert.Equal(summary, activity.SummarizedContent);
        Assert.NotNull(activity.ProcessedAt);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (DailyActivity)Activator.CreateInstance(typeof(DailyActivity), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
