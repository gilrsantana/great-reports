using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ClientContacts.Commands;
using GreatReports.Application.UseCases.ClientContacts.CommandHandlers;
using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using Moq;

namespace GreatReports.UnitTests.Application;

public class AddClientContactCommandHandlerTests
{
    private readonly Mock<IClientCompanyRepository> _clientCompanyRepositoryMock = new();
    private readonly Mock<IClientContactRepository> _clientContactRepositoryMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly AddClientContactCommandHandler _handler;

    public AddClientContactCommandHandlerTests()
    {
        _handler = new AddClientContactCommandHandler(
            _clientCompanyRepositoryMock.Object,
            _clientContactRepositoryMock.Object,
            _identityServiceMock.Object);
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
    public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExists()
    {
        // Arrange
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var command = new AddClientContactCommand(clientCompany.Id, "Maria Souza", "maria@email.com", "Commercial");
        var existingContact = ClientContact.Create(clientCompany.Id, "Maria Souza", "maria@email.com", ContactType.Commercial).Value;

        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);

        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingContact);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientContact.EmailAlreadyExists", result.Error.Code);
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

        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientContact?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientContact.InvalidName", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldRollbackAndReturnFailure_WhenAccountRegistrationFails()
    {
        // Arrange
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var command = new AddClientContactCommand(clientCompany.Id, "Maria Souza", "maria@email.com", "Commercial");

        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);

        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientContact?)null);

        _identityServiceMock
            .Setup(x => x.CreateUserAsync(
                It.IsAny<Guid>(),
                command.Email,
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(r => r.Contains("Stakeholder"))))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientContact.RegistrationRollback", result.Error.Code);

        _clientContactRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ClientContact>(), It.IsAny<CancellationToken>()), Times.Once);
        _clientContactRepositoryMock.Verify(x => x.Delete(It.IsAny<ClientContact>()), Times.Once);
        _clientContactRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
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

        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientContact?)null);

        _identityServiceMock
            .Setup(x => x.CreateUserAsync(
                It.IsAny<Guid>(),
                command.Email,
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(r => r.Contains("Stakeholder"))))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        _clientContactRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ClientContact>(), It.IsAny<CancellationToken>()), Times.Once);
        _clientContactRepositoryMock.Verify(x => x.Update(It.IsAny<ClientContact>()), Times.Never);
        _clientContactRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
