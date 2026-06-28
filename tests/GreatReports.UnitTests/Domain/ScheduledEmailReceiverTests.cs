using GreatReports.Domain.Entities;
using System;
using Xunit;

namespace GreatReports.UnitTests.Domain;

public class ScheduledEmailReceiverTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenScheduledEmailIdIsEmpty()
    {
        // Act
        var result = ScheduledEmailReceiver.Create(Guid.Empty, Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ScheduledEmailReceiver.InvalidScheduledEmail", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenUserIdIsEmpty()
    {
        // Act
        var result = ScheduledEmailReceiver.Create(Guid.NewGuid(), Guid.Empty);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ScheduledEmailReceiver.InvalidUser", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Arrange
        var emailId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var contactId = Guid.NewGuid();

        // Act
        var result = ScheduledEmailReceiver.Create(emailId, userId, contactId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(emailId, result.Value.ScheduledEmailId);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(contactId, result.Value.ClientContactId);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (ScheduledEmailReceiver)Activator.CreateInstance(typeof(ScheduledEmailReceiver), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
