using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using System;
using Xunit;

namespace GreatReports.UnitTests.Domain;

public class ClientContactTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenClientCompanyIdIsEmpty()
    {
        // Act
        var result = ClientContact.Create(Guid.Empty, "Maria Souza", "maria@provedor.com", ContactType.Commercial);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientContact.InvalidClientCompany", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Act
        var result = ClientContact.Create(Guid.NewGuid(), "", "maria@provedor.com", ContactType.Commercial);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientContact.InvalidName", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenEmailIsEmpty()
    {
        // Act
        var result = ClientContact.Create(Guid.NewGuid(), "Maria Souza", "", ContactType.Commercial);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientContact.InvalidEmail", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenEmailIsInvalid()
    {
        // Act
        var result = ClientContact.Create(Guid.NewGuid(), "Maria Souza", "maria@", ContactType.Commercial);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientContact.InvalidEmailFormat", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Arrange
        var clientCompanyId = Guid.NewGuid();
        var name = "Maria Souza";
        var email = "maria@provedor.com";
        var type = ContactType.Commercial;

        // Act
        var result = ClientContact.Create(clientCompanyId, name, email, type);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(clientCompanyId, result.Value.ClientCompanyId);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(type, result.Value.Type);
        Assert.False(result.Value.EmailConfirmed);
        Assert.Null(result.Value.VerificationToken);
    }

    [Fact]
    public void GenerateVerificationToken_ShouldSetToken_WhenCalled()
    {
        // Arrange
        var contact = ClientContact.Create(Guid.NewGuid(), "Maria Souza", "maria@provedor.com", ContactType.Commercial).Value;

        // Act
        contact.GenerateVerificationToken();

        // Assert
        Assert.NotNull(contact.VerificationToken);
        Assert.NotEmpty(contact.VerificationToken);
    }

    [Fact]
    public void ConfirmEmail_ShouldClearTokenAndSetConfirmedTrue_WhenCalled()
    {
        // Arrange
        var contact = ClientContact.Create(Guid.NewGuid(), "Maria Souza", "maria@provedor.com", ContactType.Commercial).Value;
        contact.GenerateVerificationToken();

        // Act
        contact.ConfirmEmail();

        // Assert
        Assert.True(contact.EmailConfirmed);
        Assert.Null(contact.VerificationToken);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (ClientContact)Activator.CreateInstance(typeof(ClientContact), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
