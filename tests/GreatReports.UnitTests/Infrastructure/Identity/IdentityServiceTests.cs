using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Responses;
using GreatReports.Infrastructure.Configurations;
using GreatReports.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GreatReports.UnitTests.Infrastructure.Identity;

public class IdentityServiceTests
{
    private readonly Mock<UserManager<Account>> _userManagerMock;
    private readonly Mock<RoleManager<Role>> _roleManagerMock;
    private readonly Mock<IEmailSender<Account>> _emailSenderMock;
    private readonly Mock<ILogger<IdentityService>> _loggerMock = new();
    private readonly IOptions<JwtSettings> _jwtOptions;
    private readonly IOptions<ClientSettings> _clientOptions;
    private readonly IdentityService _service;

    public IdentityServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<Account>>();
        _userManagerMock = new Mock<UserManager<Account>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var roleStoreMock = new Mock<IRoleStore<Role>>();
        _roleManagerMock = new Mock<RoleManager<Role>>(
            roleStoreMock.Object, null!, null!, null!, null!);

        _emailSenderMock = new Mock<IEmailSender<Account>>();

        var jwtSettings = new JwtSettings
        {
            Secret = "SuperSecretKeyForTestingSuperSecretKeyForTesting",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryInMinutes = 15,
            RefreshTokenExpireInDays = 7,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
        _jwtOptions = Options.Create(jwtSettings);

        var clientSettings = new ClientSettings
        {
            BaseUrl = "https://localhost:4200"
        };
        _clientOptions = Options.Create(clientSettings);

        _service = new IdentityService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _jwtOptions,
            _emailSenderMock.Object,
            _clientOptions,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnTrue_WhenCreationSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "newuser@test.com";
        var password = "Password123!";
        var roles = new[] { "User" };

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<Account>(), password))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock
            .Setup(x => x.RoleExistsAsync("User"))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<Account>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<Account>()))
            .ReturnsAsync("confirm-token");

        // Act
        var result = await _service.CreateUserAsync(id, email, password, roles);

        // Assert
        Assert.True(result);
        _emailSenderMock.Verify(x => x.SendConfirmationLinkAsync(It.IsAny<Account>(), email, It.Is<string>(link => link.Contains("confirm-email"))), Times.Once);
    }

    [Fact]
    public async Task UpdateLockoutStatusAsync_ShouldReturnTrue_WhenUpdateSucceeds()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = Account.Create(accountId, "test@test.com");

        _userManagerMock
            .Setup(x => x.FindByIdAsync(accountId.ToString()))
            .ReturnsAsync(account);

        _userManagerMock
            .Setup(x => x.UpdateAsync(account))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.UpdateLockoutStatusAsync(accountId, true);

        // Assert
        Assert.True(result);
        Assert.NotNull(account.LockoutEnd);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnTrue_WhenConfirmationSucceeds()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = Account.Create(accountId, "test@test.com");
        var token = "token";

        _userManagerMock
            .Setup(x => x.FindByIdAsync(accountId.ToString()))
            .ReturnsAsync(account);

        _userManagerMock
            .Setup(x => x.ConfirmEmailAsync(account, token))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.ConfirmEmailAsync(accountId, token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        // Act
        var result = await _service.AuthenticateAsync("notfound@test.com", "pass");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnFailure_WhenPasswordInvalid()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), "test@test.com");
        _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(account);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(account, "wrong")).ReturnsAsync(false);

        // Act
        var result = await _service.AuthenticateAsync("test@test.com", "wrong");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnFailure_WhenEmailNotConfirmed()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), "test@test.com"); // EmailConfirmed is false by default
        _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(account);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(account, "correct")).ReturnsAsync(true);

        // Act
        var result = await _service.AuthenticateAsync("test@test.com", "correct");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.EmailNotConfirmed", result.Error.Code);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnFailure_WhenAccountLocked()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), "test@test.com");
        account.EmailConfirmed = true;
        account.UpdateLockoutStatus(true);
        _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(account);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(account, "correct")).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.IsLockedOutAsync(account)).ReturnsAsync(true);

        // Act
        var result = await _service.AuthenticateAsync("test@test.com", "correct");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.AccountLocked", result.Error.Code);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnTokens_WhenCredentialsAreValid()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), "test@test.com");
        account.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(account);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(account, "correct")).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.GetRolesAsync(account)).ReturnsAsync(new List<string> { "User" });
        _userManagerMock.Setup(x => x.UpdateAsync(account)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.AuthenticateAsync("test@test.com", "correct");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.AccessToken);
        Assert.NotNull(result.Value.RefreshToken);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnSuccess_WhenSuccessfullyChanged()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = Account.Create(accountId, "test@test.com");
        _userManagerMock.Setup(x => x.FindByIdAsync(accountId.ToString())).ReturnsAsync(account);
        _userManagerMock.Setup(x => x.ChangePasswordAsync(account, "Old!", "New!")).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.ChangePasswordAsync(accountId, "Old!", "New!");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GeneratePasswordResetTokenAsync_ShouldReturnSuccess_AndSendEmail_WhenUserExists()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), "test@test.com");
        _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(account);
        _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(account)).ReturnsAsync("reset-token");

        // Act
        var result = await _service.GeneratePasswordResetTokenAsync("test@test.com");

        // Assert
        Assert.True(result.IsSuccess);
        _emailSenderMock.Verify(x => x.SendPasswordResetLinkAsync(account, "test@test.com", It.Is<string>(link => link.Contains("reset-password"))), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnSuccess_WhenTokenIsValid()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), "test@test.com");
        _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(account);
        _userManagerMock.Setup(x => x.ResetPasswordAsync(account, "token", "New!")).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.ResetPasswordAsync("test@test.com", "token", "New!");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task RotateTokensAsync_ShouldReturnNewTokens_WhenValid()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = Account.Create(accountId, "test@test.com");
        var refreshToken = "valid-refresh-token";
        account.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(1));

        // Generate a token
        var claims = new List<Claim>
        {
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, accountId.ToString()),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, "test@test.com")
        };
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtOptions.Value.Secret));
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var jwtToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: _jwtOptions.Value.Issuer,
            audience: _jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(-5), // expired but within leeway / lifetime check disabled
            signingCredentials: creds
        );
        var accessToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(jwtToken);

        _userManagerMock.Setup(x => x.FindByIdAsync(accountId.ToString())).ReturnsAsync(account);
        _userManagerMock.Setup(x => x.GetRolesAsync(account)).ReturnsAsync(new List<string>());
        _userManagerMock.Setup(x => x.UpdateAsync(account)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.RotateTokensAsync(accessToken, refreshToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Value.AccessToken);
    }
}
