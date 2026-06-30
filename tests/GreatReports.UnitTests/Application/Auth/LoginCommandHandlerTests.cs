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

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailOrPasswordIsEmpty()
    {
        // Arrange
        var command = new LoginCommand("", "");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var command = new LoginCommand("test@email.com", "Password123!");
        var expectedResponse = new TokenResponse("access_token", "refresh_token");
        
        _identityServiceMock
            .Setup(x => x.AuthenticateAsync(command.Email, command.Password))
            .ReturnsAsync(Result.Success(expectedResponse));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResponse, result.Value);
    }
}
