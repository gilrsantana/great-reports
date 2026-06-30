using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Auth.Commands;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : ICommand;
