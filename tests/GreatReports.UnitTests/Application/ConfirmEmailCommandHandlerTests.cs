using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using GreatReports.Shared;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
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
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserTokenIsInvalid()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "João Silva", "test@email.com").Value;
        user.GenerateVerificationToken();
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

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
        user.GenerateVerificationToken();
        var token = user.VerificationToken;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new ConfirmEmailCommand("test@email.com", token!);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(user.EmailConfirmed);
        Assert.Null(user.VerificationToken);
        
        _userRepositoryMock.Verify(x => x.Update(user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _identityServiceMock.Verify(x => x.ConfirmEmailAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenContactAlreadyConfirmed()
    {
        // Arrange
        var contact = ClientContact.Create(Guid.NewGuid(), "Maria", "maria@email.com", ContactType.Commercial).Value;
        contact.ConfirmEmail(); // Already confirmed

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        var command = new ConfirmEmailCommand("maria@email.com", "any-token");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _clientContactRepositoryMock.Verify(x => x.Update(It.IsAny<ClientContact>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenContactTokenIsInvalid()
    {
        // Arrange
        var contact = ClientContact.Create(Guid.NewGuid(), "Maria", "maria@email.com", ContactType.Commercial).Value;
        contact.GenerateVerificationToken();

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        var command = new ConfirmEmailCommand("maria@email.com", "wrong-token");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidToken", result.Error.Code);
        _clientContactRepositoryMock.Verify(x => x.Update(It.IsAny<ClientContact>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_AndConfirmEmail_WhenContactTokenIsValid()
    {
        // Arrange
        var contact = ClientContact.Create(Guid.NewGuid(), "Maria", "maria@email.com", ContactType.Commercial).Value;
        contact.GenerateVerificationToken();
        var token = contact.VerificationToken;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _clientContactRepositoryMock
            .Setup(x => x.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        var command = new ConfirmEmailCommand("maria@email.com", token!);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(contact.EmailConfirmed);
        Assert.Null(contact.VerificationToken);

        _clientContactRepositoryMock.Verify(x => x.Update(contact), Times.Once);
        _clientContactRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
