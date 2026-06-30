using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Auth.Commands;

public record ForgetPasswordCommand(string Email) : ICommand;
