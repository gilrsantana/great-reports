using GreatReports.Domain.Entities;

namespace GreatReports.UnitTests.Domain;

public class ProviderCompanyTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Act
        var result = ProviderCompany.Create("", "12.345.678/0001-90", Guid.CreateVersion7());

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ProviderCompany.InvalidName", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenTaxIdIsEmpty()
    {
        // Act
        var result = ProviderCompany.Create("Provedor Teste", "", Guid.CreateVersion7());

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ProviderCompany.InvalidTaxId", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Arrange
        var name = "Provedor Teste";
        var taxId = "12.345.678/0001-90";

        // Act
        var result = ProviderCompany.Create(name, taxId, Guid.CreateVersion7());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(taxId, result.Value.TaxId);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (ProviderCompany)Activator.CreateInstance(typeof(ProviderCompany), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
