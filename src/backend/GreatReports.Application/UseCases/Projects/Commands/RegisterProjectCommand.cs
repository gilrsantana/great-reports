using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Projects.Commands;

public record RegisterProjectCommand(Guid ClientCompanyId, string Name, string Description) : ICommand<Guid>;
