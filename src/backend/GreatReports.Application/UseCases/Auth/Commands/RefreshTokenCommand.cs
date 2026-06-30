using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Auth.Responses;

namespace GreatReports.Application.UseCases.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<TokenResponse>;
