using GreatReports.Application.UseCases.Auth.Responses;
using GreatReports.Shared;

namespace GreatReports.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> CreateUserAsync(Guid id, string email, string password, IEnumerable<string> roles);
    Task<bool> UpdateLockoutStatusAsync(Guid accountId, bool blocked);
    Task<bool> ConfirmEmailAsync(Guid accountId, string token);
    Task<Result<TokenResponse>> AuthenticateAsync(string email, string password);
    Task<Result> ChangePasswordAsync(Guid accountId, string currentPassword, string newPassword);
    Task<Result> GeneratePasswordResetTokenAsync(string email);
    Task<Result> ResetPasswordAsync(string email, string token, string newPassword);
    Task<Result<TokenResponse>?> RotateTokensAsync(string accessToken, string refreshToken);
    Task<Dictionary<Guid, string>> GetUserRolesAsync(IEnumerable<Guid> userIds);
}
