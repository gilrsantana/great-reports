using System;
using System.Threading;
using System.Threading.Tasks;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Application.UseCases.Auth.CommandHandlers;
using GreatReports.Shared;
using Moq;
using Xunit;

namespace GreatReports.UnitTests.Application.Auth;

public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _handler = new ChangePasswordCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPasswordsAreEmpty()
    {
        // Arrange
        var command = new ChangePasswordCommand(Guid.NewGuid(), "", "");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidPassword", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenPasswordChangedSuccessfully()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var command = new ChangePasswordCommand(accountId, "OldPassword!", "NewPassword!");
        _identityServiceMock
            .Setup(x => x.ChangePasswordAsync(accountId, command.CurrentPassword, command.NewPassword))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
