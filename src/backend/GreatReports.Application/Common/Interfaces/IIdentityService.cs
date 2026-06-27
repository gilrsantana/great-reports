namespace GreatReports.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(string AccessToken, string RefreshToken)?> GenerateTokensAsync(Guid accountId, string email, IEnumerable<string> roles);
    Task<(string AccessToken, string RefreshToken)?> RotateTokensAsync(string accessToken, string refreshToken);
    Task<bool> CreateUserAsync(Guid id, string email, string password, IEnumerable<string> roles);
    Task<bool> UpdateLockoutStatusAsync(Guid accountId, bool blocked);
}
