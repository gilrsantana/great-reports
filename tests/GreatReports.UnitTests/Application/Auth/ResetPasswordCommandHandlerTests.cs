using System.Threading;
using System.Threading.Tasks;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Application.UseCases.Auth.CommandHandlers;
using GreatReports.Shared;
using Moq;
using Xunit;

namespace GreatReports.UnitTests.Application.Auth;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _handler = new ResetPasswordCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenFieldsAreEmpty()
    {
        // Arrange
        var command = new ResetPasswordCommand("", "", "");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidResetRequest", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenResetIsSuccessful()
    {
        // Arrange
        var command = new ResetPasswordCommand("test@email.com", "valid_token", "NewPassword123!");
        _identityServiceMock
            .Setup(x => x.ResetPasswordAsync(command.Email, command.Token, command.NewPassword))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
