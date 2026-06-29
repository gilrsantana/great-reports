using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Users.Commands;

public record RegisterUserCommand(Guid ProviderCompanyId, string DisplayName, string Email, string Role) : ICommand<Guid>;
