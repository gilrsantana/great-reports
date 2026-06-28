using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ProviderCompanies.Queries;
using GreatReports.Domain.Entities;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GreatReports.UnitTests.Application;

public class GetProviderDetailsQueryHandlerTests
{
    private readonly Mock<IProviderCompanyRepository> _providerCompanyRepositoryMock = new();
    private readonly GetProviderDetailsQueryHandler _handler;

    public GetProviderDetailsQueryHandlerTests()
    {
        _handler = new GetProviderDetailsQueryHandler(_providerCompanyRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProviderNotFound()
    {
        // Arrange
        var query = new GetProviderDetailsQuery(Guid.NewGuid());
        _providerCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(query.ProviderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProviderCompany?)null);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ProviderCompany.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProviderExists()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var provider = ProviderCompany.Create("Provedor Teste", "12.345.678/0001-90").Value;
        // Since provider ID is set privately by BaseEntity, let's look at how we can mock it or if the entity constructor creates it.
        // Wait, BaseEntity has a public or protected constructor that generates a Guid? Let's check BaseEntity.cs.
        // Let's inspect BaseEntity.cs. Let's do that!
        
        // Actually, let's look at BaseEntity definition if needed, but normally ID is Guid.NewGuid() in the BaseEntity constructor. Let's verify.
        // Yes, BaseEntity usually assigns `Id = Guid.NewGuid()` in constructor. So provider.Id will be a random Guid. Let's use provider.Id as query.ProviderId.
        
        var query = new GetProviderDetailsQuery(provider.Id);
        _providerCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(provider.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(provider.Id, result.Value.Id);
        Assert.Equal(provider.Name, result.Value.Name);
        Assert.Equal(provider.TaxId, result.Value.TaxId);
    }
}
