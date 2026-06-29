using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GreatReports.Infrastructure.Identity;

public class IdentityService(
    UserManager<Account> userManager,
    RoleManager<Role> roleManager,
    IOptions<JwtSettings> jwtSettings,
    IEmailSender<Account> emailSender,
    IOptions<ClientSettings> clientSettings) : IIdentityService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly ClientSettings _clientSettings = clientSettings.Value;

    public async Task<(string AccessToken, string RefreshToken)?> GenerateTokensAsync(Guid accountId, string email, IEnumerable<string> roles)
    {
        var account = await userManager.FindByIdAsync(accountId.ToString());
        if (account == null) return null;

        var accessToken = GenerateAccessToken(accountId, email, roles);
        var refreshToken = GenerateRefreshToken();

        account.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireInDays));
        await userManager.UpdateAsync(account);

        return (accessToken, refreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)?> RotateTokensAsync(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        var emailClaim = principal.FindFirst(ClaimTypes.Email) ?? principal.FindFirst(JwtRegisteredClaimNames.Email);
        if (emailClaim == null) return null;

        var account = await userManager.FindByEmailAsync(emailClaim.Value);
        if (account == null || account.RefreshToken != refreshToken || account.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(account);
        var newAccessToken = GenerateAccessToken(account.Id, account.Email!, roles);
        var newRefreshToken = GenerateRefreshToken();

        account.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireInDays));
        await userManager.UpdateAsync(account);

        return (newAccessToken, newRefreshToken);
    }

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

    public async Task<bool> ConfirmEmailAsync(Guid accountId, string token)
    {
        var account = await userManager.FindByIdAsync(accountId.ToString());
        if (account == null) return false;

        var result = await userManager.ConfirmEmailAsync(account, token);
        return result.Succeeded;
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
            ValidateLifetime = false
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
