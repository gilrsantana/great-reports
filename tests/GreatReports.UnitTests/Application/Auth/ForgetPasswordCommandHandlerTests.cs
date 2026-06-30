using System.Threading;
using System.Threading.Tasks;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Application.UseCases.Auth.CommandHandlers;
using GreatReports.Shared;
using Moq;
using Xunit;

namespace GreatReports.UnitTests.Application.Auth;

public class ForgetPasswordCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly ForgetPasswordCommandHandler _handler;

    public ForgetPasswordCommandHandlerTests()
    {
        _handler = new ForgetPasswordCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new ForgetPasswordCommand("");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidEmail", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEmailIsValid()
    {
        // Arrange
        var command = new ForgetPasswordCommand("test@email.com");
        _identityServiceMock
            .Setup(x => x.GeneratePasswordResetTokenAsync(command.Email))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
