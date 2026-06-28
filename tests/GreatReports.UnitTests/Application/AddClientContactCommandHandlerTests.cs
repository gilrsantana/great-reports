using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ClientContacts.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GreatReports.UnitTests.Application;

public class AddClientContactCommandHandlerTests
{
    private readonly Mock<IClientCompanyRepository> _clientCompanyRepositoryMock = new();
    private readonly Mock<IClientContactRepository> _clientContactRepositoryMock = new();
    private readonly Mock<IEmailVerificationService> _emailVerificationServiceMock = new();
    private readonly AddClientContactCommandHandler _handler;

    public AddClientContactCommandHandlerTests()
    {
        _handler = new AddClientContactCommandHandler(
            _clientCompanyRepositoryMock.Object,
            _clientContactRepositoryMock.Object,
            _emailVerificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenClientCompanyNotFound()
    {
        // Arrange
        var command = new AddClientContactCommand(Guid.NewGuid(), "Maria Souza", "maria@email.com", "Commercial");
        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientCompany?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientCompany.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenContactTypeIsInvalid()
    {
        // Arrange
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var command = new AddClientContactCommand(clientCompany.Id, "Maria Souza", "maria@email.com", "InvalidType");

        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientContact.InvalidContactType", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenContactCreationFails()
    {
        // Arrange
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var command = new AddClientContactCommand(clientCompany.Id, "", "maria@email.com", "Commercial"); // Empty name fails creation

        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientContact.InvalidName", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var command = new AddClientContactCommand(clientCompany.Id, "Maria Souza", "maria@email.com", "Commercial");

        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        _clientContactRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ClientContact>(), It.IsAny<CancellationToken>()), Times.Once);
        _clientContactRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _emailVerificationServiceMock.Verify(x => x.SendVerificationEmailAsync("maria@email.com", It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
