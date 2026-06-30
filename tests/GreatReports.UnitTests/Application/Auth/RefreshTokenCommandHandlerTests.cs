using System.Threading;
using System.Threading.Tasks;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Application.UseCases.Auth.CommandHandlers;
using GreatReports.Application.UseCases.Auth.Responses;
using GreatReports.Shared;
using Moq;
using Xunit;

namespace GreatReports.UnitTests.Application.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _handler = new RefreshTokenCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTokensAreEmpty()
    {
        // Arrange
        var command = new RefreshTokenCommand("", "");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidToken", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRotationFails()
    {
        // Arrange
        var command = new RefreshTokenCommand("expired_access", "invalid_refresh");
        _identityServiceMock
            .Setup(x => x.RotateTokensAsync(command.AccessToken, command.RefreshToken))
            .ReturnsAsync((Result<TokenResponse>?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidRefreshToken", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTokensAreRotatedSuccessfully()
    {
        // Arrange
        var command = new RefreshTokenCommand("expired_access", "valid_refresh");
        var expectedResponse = new TokenResponse("new_access", "new_refresh");
        _identityServiceMock
            .Setup(x => x.RotateTokensAsync(command.AccessToken, command.RefreshToken))
            .ReturnsAsync(Result.Success(expectedResponse));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResponse.AccessToken, result.Value.AccessToken);
        Assert.Equal(expectedResponse.RefreshToken, result.Value.RefreshToken);
    }
}
