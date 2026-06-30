using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Application.UseCases.Auth.Responses;
using GreatReports.Presentation.Requests;
using GreatReports.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GreatReports.IntegrationTests;

public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<ICommandHandler<ConfirmEmailCommand>> _confirmEmailHandlerMock = new();
    private readonly Mock<ICommandHandler<LoginCommand, TokenResponse>> _loginHandlerMock = new();
    private readonly Mock<ICommandHandler<RefreshTokenCommand, TokenResponse>> _refreshTokenHandlerMock = new();
    private readonly Mock<ICommandHandler<ChangePasswordCommand>> _changePasswordHandlerMock = new();
    private readonly Mock<ICommandHandler<ForgetPasswordCommand>> _forgetPasswordHandlerMock = new();
    private readonly Mock<ICommandHandler<ResetPasswordCommand>> _resetPasswordHandlerMock = new();

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove Hangfire hosted services to avoid database connection attempts
                var hangfireHosted = services.Where(d =>
                    d.ImplementationType?.FullName?.Contains("Hangfire") == true).ToList();
                foreach (var service in hangfireHosted)
                {
                    services.Remove(service);
                }

                // Replace CQRS handlers with our mocks
                ReplaceService(services, _confirmEmailHandlerMock.Object);
                ReplaceService(services, _loginHandlerMock.Object);
                ReplaceService(services, _refreshTokenHandlerMock.Object);
                ReplaceService(services, _changePasswordHandlerMock.Object);
                ReplaceService(services, _forgetPasswordHandlerMock.Object);
                ReplaceService(services, _resetPasswordHandlerMock.Object);
            });
        });
    }

    private static void ReplaceService<TService>(IServiceCollection services, TService implementation)
        where TService : class
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(TService)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
        services.AddScoped(_ => implementation);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequest("test@test.com", "Password123!");
        var expectedResponse = new TokenResponse("valid_access_token", "valid_refresh_token");

        _loginHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedResponse));

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(content);
        Assert.Equal("valid_access_token", content.AccessToken);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequest("test@test.com", "WrongPassword");

        _loginHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<TokenResponse>(new Error("Auth.InvalidCredentials", "E-mail ou senha incorretos.")));

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenTokenIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new RefreshTokenRequest("expired_access", "valid_refresh");
        var expectedResponse = new TokenResponse("new_access", "new_refresh");

        _refreshTokenHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedResponse));

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/refresh-token", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(content);
        Assert.Equal("new_access", content.AccessToken);
    }

    [Fact]
    public async Task ForgetPassword_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new ForgetPasswordRequest("test@test.com");

        _forgetPasswordHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<ForgetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/forget-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new ResetPasswordRequest("test@test.com", "token", "NewPassword123!");

        _resetPasswordHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/reset-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
