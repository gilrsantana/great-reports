using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Auth.Responses;

namespace GreatReports.Application.UseCases.Auth.Commands;

public record LoginCommand(string Email, string Password) : ICommand<TokenResponse>;
