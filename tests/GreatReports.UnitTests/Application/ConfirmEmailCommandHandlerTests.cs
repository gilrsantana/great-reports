using System;
using System.Threading;
using System.Threading.Tasks;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Application.UseCases.Auth.CommandHandlers;
using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using Moq;
using Xunit;

namespace GreatReports.UnitTests.Application;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IClientContactRepository> _clientContactRepositoryMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly ConfirmEmailCommandHandler _handler;

    public ConfirmEmailCommandHandlerTests()
    {
        _handler = new ConfirmEmailCommandHandler(
            _userRepositoryMock.Object,
            _clientContactRepositoryMock.Object,
            _identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserAlreadyConfirmed()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "João Silva", "test@email.com").Value;
        user.ConfirmEmail(); // Already confirmed

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new ConfirmEmailCommand("test@email.com", "any-token");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        _identityServiceMock.Verify(x => x.ConfirmEmailAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserTokenIsInvalid()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "João Silva", "test@email.com").Value;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _identityServiceMock
            .Setup(x => x.ConfirmEmailAsync(user.Id, "wrong-token"))
            .ReturnsAsync(false);

        var command = new ConfirmEmailCommand("test@email.com", "wrong-token");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidToken", result.Error.Code);
        _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_AndConfirmEmail_WhenUserTokenIsValid()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "João Silva", "test@email.com").Value;
        var token = "valid-token";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _identityServiceMock
            .Setup(x => x.ConfirmEmailAsync(user.Id, token))
            .ReturnsAsync(true);

        var command = new ConfirmEmailCommand("test@email.com", token);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(user.EmailConfirmed);

        _userRepositoryMock.Verify(x => x.Update(user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _identityServiceMock.Verify(x => x.ConfirmEmailAsync(user.Id, token), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenContactTokenIsInvalid()
    {
        // Arrange
        var contact = ClientContact.Create(Guid.NewGuid(), "Maria", "maria@email.com", ContactType.Commercial).Value;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        _identityServiceMock
            .Setup(x => x.ConfirmEmailAsync(contact.Id, "wrong-token"))
            .ReturnsAsync(false);

        var command = new ConfirmEmailCommand("maria@email.com", "wrong-token");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidToken", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_AndConfirmEmail_WhenContactTokenIsValid()
    {
        // Arrange
        var contact = ClientContact.Create(Guid.NewGuid(), "Maria", "maria@email.com", ContactType.Commercial).Value;
        var token = "valid-token";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        _identityServiceMock
            .Setup(x => x.ConfirmEmailAsync(contact.Id, token))
            .ReturnsAsync(true);

        var command = new ConfirmEmailCommand("maria@email.com", token);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _identityServiceMock.Verify(x => x.ConfirmEmailAsync(contact.Id, token), Times.Once);
        _clientContactRepositoryMock.Verify(x => x.Update(It.IsAny<ClientContact>()), Times.Never);
        _clientContactRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserAndContactNotFound()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("notfound@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync("notfound@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientContact?)null);

        var command = new ConfirmEmailCommand("notfound@email.com", "token");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.UserNotFound", result.Error.Code);
    }
}
