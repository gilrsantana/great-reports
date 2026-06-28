using GreatReports.Domain.Entities;
using System;
using Xunit;

namespace GreatReports.UnitTests.Domain;

public class ClientCompanyTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenProviderCompanyIdIsEmpty()
    {
        // Act
        var result = ClientCompany.Create(Guid.Empty, "Empresa Cliente");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientCompany.InvalidProvider", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Act
        var result = ClientCompany.Create(Guid.NewGuid(), "");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientCompany.InvalidName", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var name = "Empresa Cliente";

        // Act
        var result = ClientCompany.Create(providerId, name);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(providerId, result.Value.ProviderCompanyId);
        Assert.Equal(name, result.Value.Name);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (ClientCompany)Activator.CreateInstance(typeof(ClientCompany), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
