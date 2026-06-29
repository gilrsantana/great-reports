using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hangfire.Dashboard;
using Microsoft.IdentityModel.Tokens;

namespace GreatReports.Presentation.Filters;

public class HangfireDashboardAuthorizationFilter(string jwtSecret, string issuer, string audience) : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // 1. Retrieve the token from custom cookie or query string parameter
        if (!httpContext.Request.Cookies.TryGetValue("HangfireToken", out var token) || string.IsNullOrEmpty(token))
        {
            token = httpContext.Request.Query["token"];
        }

        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        // 2. Validate the JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // 3. Assert correct role permissions (Maintainer role has access)
            return principal.IsInRole("Maintainer");
        }
        catch
        {
            return false; // Authentication/Authorization failed
        }
    }
}
