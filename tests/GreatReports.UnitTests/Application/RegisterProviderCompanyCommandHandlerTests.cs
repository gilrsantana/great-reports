using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ProviderCompanies.CommandHandlers;
using GreatReports.Application.UseCases.ProviderCompanies.Commands;
using GreatReports.Domain.Entities;
using Moq;

namespace GreatReports.UnitTests.Application;

public class RegisterProviderCompanyCommandHandlerTests
{
    private readonly Mock<IProviderCompanyRepository> _providerCompanyRepositoryMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly RegisterProviderCompanyCommandHandler _handler;

    public RegisterProviderCompanyCommandHandlerTests()
    {
        _handler = new RegisterProviderCompanyCommandHandler(_providerCompanyRepositoryMock.Object, _identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNameOrTaxIdIsInvalid()
    {
        // Arrange
        var command = new RegisterProviderCompanyCommand("", "12.345.678/0001-90", Guid.CreateVersion7());

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ProviderCompany.InvalidName", result.Error.Code);
        _providerCompanyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ProviderCompany>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTaxIdAlreadyExists()
    {
        // Arrange
        var command = new RegisterProviderCompanyCommand("Provedor Teste", "12.345.678/0001-90", Guid.CreateVersion7());

        _providerCompanyRepositoryMock
            .Setup(x => x.ExistsByTaxIdAsync(command.TaxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ProviderCompany.TaxIdAlreadyExists", result.Error.Code);
        _providerCompanyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ProviderCompany>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var command = new RegisterProviderCompanyCommand("Provedor Teste", "12.345.678/0001-90", Guid.CreateVersion7());

        _providerCompanyRepositoryMock
            .Setup(x => x.ExistsByTaxIdAsync(command.TaxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _providerCompanyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ProviderCompany>(), It.IsAny<CancellationToken>()), Times.Once);
        _providerCompanyRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
