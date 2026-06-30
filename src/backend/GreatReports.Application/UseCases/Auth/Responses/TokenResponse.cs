namespace GreatReports.Application.UseCases.Auth.Responses;

public record TokenResponse(string AccessToken, string RefreshToken);
