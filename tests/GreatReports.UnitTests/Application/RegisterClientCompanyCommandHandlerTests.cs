using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ClientCompanies.Commands;
using GreatReports.Application.UseCases.ClientCompanies.CommandHandlers;
using GreatReports.Domain.Entities;
using Moq;

namespace GreatReports.UnitTests.Application;

public class RegisterClientCompanyCommandHandlerTests
{
    private readonly Mock<IClientCompanyRepository> _clientCompanyRepositoryMock = new();
    private readonly Mock<IProviderCompanyRepository> _providerCompanyRepositoryMock = new();
    private readonly RegisterClientCompanyCommandHandler _handler;

    public RegisterClientCompanyCommandHandlerTests()
    {
        _handler = new RegisterClientCompanyCommandHandler(
            _clientCompanyRepositoryMock.Object,
            _providerCompanyRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProviderCompanyNotFound()
    {
        // Arrange
        var command = new RegisterClientCompanyCommand(Guid.NewGuid(), "Client Name");
        _providerCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ProviderCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProviderCompany?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ProviderCompany.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenClientCompanyCreationFails()
    {
        // Arrange
        var provider = ProviderCompany.Create("Provider", "Tax", Guid.CreateVersion7()).Value;
        var command = new RegisterClientCompanyCommand(provider.Id, ""); // Empty name will fail creation

        _providerCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ProviderCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientCompany.InvalidName", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var provider = ProviderCompany.Create("Provider", "Tax", Guid.CreateVersion7()).Value;
        var command = new RegisterClientCompanyCommand(provider.Id, "Client Name");

        _providerCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ProviderCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _clientCompanyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ClientCompany>(), It.IsAny<CancellationToken>()), Times.Once);
        _clientCompanyRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
