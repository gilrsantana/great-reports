using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Responses;
using GreatReports.Shared;
using GreatReports.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace GreatReports.Infrastructure.Identity;

public class IdentityService(
    UserManager<Account> userManager,
    RoleManager<Role> roleManager,
    IOptions<JwtSettings> jwtSettings,
    IEmailSender<Account> emailSender,
    IOptions<ClientSettings> clientSettings,
    Microsoft.Extensions.Logging.ILogger<IdentityService> logger) : IIdentityService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly ClientSettings _clientSettings = clientSettings.Value;

    public async Task<bool> CreateUserAsync(Guid id, string email, string password, IEnumerable<string> roles)
    {
        var account = Account.Create(id, email);
        var result = await userManager.CreateAsync(account, password);
        if (!result.Succeeded) return false;

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(Role.Create(roleName, $"{roleName} role"));
            }
            await userManager.AddToRoleAsync(account, roleName);
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(account);
        var baseUrl = _clientSettings.BaseUrl.TrimEnd('/');
        var link = $"{baseUrl}/confirm-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
        await emailSender.SendConfirmationLinkAsync(account, email, link);

        return true;
    }

    public async Task<bool> UpdateLockoutStatusAsync(Guid accountId, bool blocked)
    {
        var account = await userManager.FindByIdAsync(accountId.ToString());
        if (account == null) return false;

        account.UpdateLockoutStatus(blocked);
        var result = await userManager.UpdateAsync(account);
        return result.Succeeded;
    }

    public async Task<Result<TokenResponse>> AuthenticateAsync(string email, string password)
    {
        var account = await userManager.FindByEmailAsync(email);
        if (account == null)
        {
            return Result<TokenResponse>.Failure(new Error("Auth.InvalidCredentials", "E-mail ou senha incorretos."));
        }

        var passwordValid = await userManager.CheckPasswordAsync(account, password);
        if (!passwordValid)
        {
            return Result<TokenResponse>.Failure(new Error("Auth.InvalidCredentials", "E-mail ou senha incorretos."));
        }

        if (!account.EmailConfirmed)
        {
            return Result<TokenResponse>.Failure(new Error("Auth.EmailNotConfirmed", "O e-mail da conta ainda não foi confirmado."));
        }

        var isLockedOut = await userManager.IsLockedOutAsync(account) || (account.LockoutEnd.HasValue && account.LockoutEnd.Value > DateTimeOffset.UtcNow);
        if (isLockedOut)
        {
            return Result<TokenResponse>.Failure(new Error("Auth.AccountLocked", "Esta conta está bloqueada."));
        }

        var roles = await userManager.GetRolesAsync(account);
        var tokensResult = await GenerateTokensAsync(account, roles);
        if (tokensResult.IsFailure)
        {
            return Result<TokenResponse>.Failure(tokensResult.Error);
        }

        return tokensResult;
    }

    public async Task<bool> ConfirmEmailAsync(Guid accountId, string token)
    {
        var account = await userManager.FindByIdAsync(accountId.ToString());
        if (account == null) return false;

        var result = await userManager.ConfirmEmailAsync(account, token);
        return result.Succeeded;
    }

    public async Task<Result> ChangePasswordAsync(Guid accountId, string currentPassword, string newPassword)
    {
        var account = await userManager.FindByIdAsync(accountId.ToString());
        if (account == null)
        {
            return Result.Failure(new Error("Auth.UserNotFound", "Conta de usuário não encontrada."));
        }

        var result = await userManager.ChangePasswordAsync(account, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToArray();
            return Result.Failure(ValidationError.FromErrors(errors));
        }

        return Result.Success();
    }

    public async Task<Result> GeneratePasswordResetTokenAsync(string email)
    {
        var account = await userManager.FindByEmailAsync(email);
        if (account == null)
        {
            logger.LogWarning("GeneratePasswordResetTokenAsync: Request made for non-existent user email {Email}.", email);
            return Result.Success();
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(account);
        var baseUrl = _clientSettings.BaseUrl.TrimEnd('/');
        var resetLink = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
        await emailSender.SendPasswordResetLinkAsync(account, email, resetLink);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var account = await userManager.FindByEmailAsync(email);
        if (account == null)
        {
            return Result.Failure(new Error("Auth.UserNotFound", "Conta de usuário não encontrada."));
        }

        var result = await userManager.ResetPasswordAsync(account, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToArray();
            return Result.Failure(ValidationError.FromErrors(errors));
        }

        return Result.Success();
    }

    public async Task<Result<TokenResponse>?> RotateTokensAsync(string accessToken, string refreshToken)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                logger.LogWarning("RotateTokensAsync: Claim NameIdentifier/Sub not found in access token.");
                return null;
            }

            var account = await userManager.FindByIdAsync(userIdClaim.Value);
            if (account == null)
            {
                logger.LogWarning("RotateTokensAsync: User not found for ID {UserId}.", userIdClaim.Value);
                return null;
            }

            if (account.RefreshToken != refreshToken)
            {
                logger.LogWarning("RotateTokensAsync: Refresh token mismatch for user {UserId}.", userIdClaim.Value);
                return null;
            }

            if (account.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                logger.LogWarning("RotateTokensAsync: Refresh token expired for user {UserId}.", userIdClaim.Value);
                return null;
            }

            var roles = await userManager.GetRolesAsync(account);
            var tokensResult = await GenerateTokensAsync(account, roles);
            if (tokensResult.IsFailure)
            {
                logger.LogWarning("RotateTokensAsync: Failed to generate new tokens: {Error}", tokensResult.Error.Description);
                return null;
            }

            return tokensResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "RotateTokensAsync: Exception occurred during token rotation.");
            return null;
        }
    }

    private async Task<Result<TokenResponse>> GenerateTokensAsync(Account account, IEnumerable<string> roles)
    {
        var accessToken = GenerateAccessToken(account.Id, account.Email!, roles);
        var refreshToken = GenerateRefreshToken();
        
        var expiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireInDays);
        account.UpdateRefreshToken(refreshToken, expiryTime);

        var result = await userManager.UpdateAsync(account);
        if (!result.Succeeded)
        {
            return Result<TokenResponse>.Failure(new Error("Auth.TokenUpdateFailed", "Erro ao atualizar token de segurança."));
        }

        return Result<TokenResponse>.Success(new TokenResponse(accessToken, refreshToken));
    }

    private string GenerateAccessToken(Guid accountId, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, accountId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = _jwtSettings.ValidateAudience,
            ValidateIssuer = _jwtSettings.ValidateIssuer,
            ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateLifetime = false,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
