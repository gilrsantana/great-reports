using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Groups.Commands;

public record CreateGroupCommand(
    Guid GroupLeaderId,
    Guid ClientCompanyId,
    Guid ProjectId,
    string Name,
    string Timezone) : ICommand<Guid>;
