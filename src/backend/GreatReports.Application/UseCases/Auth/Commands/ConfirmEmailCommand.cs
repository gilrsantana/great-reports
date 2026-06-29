using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Auth.Commands;

public record ConfirmEmailCommand(string Email, string Token) : ICommand;
