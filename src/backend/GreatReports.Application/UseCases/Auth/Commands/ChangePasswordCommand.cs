using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Auth.Commands;

public record ChangePasswordCommand(Guid AccountId, string CurrentPassword, string NewPassword) : ICommand;
