using GreatReports.Domain.Entities;

namespace GreatReports.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenProviderCompanyIdIsEmpty()
    {
        // Act
        var result = User.Create(Guid.Empty, "João Silva", "joao.silva@provedor.com");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.InvalidProviderCompany", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenDisplayNameIsEmpty()
    {
        // Act
        var result = User.Create(Guid.NewGuid(), "", "joao.silva@provedor.com");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.InvalidDisplayName", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenEmailIsEmpty()
    {
        // Act
        var result = User.Create(Guid.NewGuid(), "João Silva", "");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.InvalidEmail", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenEmailIsInvalid()
    {
        // Act
        var result = User.Create(Guid.NewGuid(), "João Silva", "invalid-email");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.InvalidEmailFormat", result.Error.Code);
        Assert.Equal("O e-mail fornecido está em um formato inválido.", result.Error.Description);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenEmailIsValid()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var email = "joao.silva@provedor.com";
        var name = "João Silva";

        // Act
        var result = User.Create(providerId, name, email);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(name, result.Value.DisplayName);
        Assert.Equal(providerId, result.Value.ProviderCompanyId);
        Assert.False(result.Value.EmailConfirmed);
    }

    [Fact]
    public void ConfirmEmail_ShouldSetConfirmedTrue_WhenCalled()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "João Silva", "joao@email.com").Value;

        // Act
        user.ConfirmEmail();

        // Assert
        Assert.True(user.EmailConfirmed);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
