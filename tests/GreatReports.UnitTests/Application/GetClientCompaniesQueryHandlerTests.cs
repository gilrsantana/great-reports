using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.Common.Models;
using GreatReports.Application.UseCases.ClientCompanies.Queries;
using GreatReports.Domain.Entities;
using GreatReports.Shared;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GreatReports.UnitTests.Application;

public class GetClientCompaniesQueryHandlerTests
{
    private readonly Mock<IClientCompanyRepository> _clientCompanyRepositoryMock = new();
    private readonly GetClientCompaniesQueryHandler _handler;

    public GetClientCompaniesQueryHandlerTests()
    {
        _handler = new GetClientCompaniesQueryHandler(_clientCompanyRepositoryMock.Object);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public async Task Handle_ShouldReturnFailure_WhenPageIsZeroOrNegative(int invalidPage)
    {
        // Arrange
        var query = new GetClientCompaniesQuery(Guid.NewGuid(), invalidPage, 10);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Query.InvalidPage", result.Error.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public async Task Handle_ShouldReturnFailure_WhenPageSizeIsZeroOrNegative(int invalidPageSize)
    {
        // Arrange
        var query = new GetClientCompaniesQuery(Guid.NewGuid(), 1, invalidPageSize);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Query.InvalidPageSize", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WithPagedItems_WhenQueryIsValid()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var query = new GetClientCompaniesQuery(providerId, 1, 10);
        
        var list = new List<ClientCompany>
        {
            ClientCompany.Create(providerId, "Client 1").Value,
            ClientCompany.Create(providerId, "Client 2").Value
        };

        _clientCompanyRepositoryMock
            .Setup(x => x.GetPagedByProviderIdAsync(providerId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((list, 2));

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(1, result.Value.Page);
        Assert.Equal(10, result.Value.PageSize);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal("Client 1", result.Value.Items[0].Name);
        Assert.Equal("Client 2", result.Value.Items[1].Name);
    }
}
