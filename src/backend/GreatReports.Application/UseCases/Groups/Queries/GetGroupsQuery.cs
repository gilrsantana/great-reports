using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Groups.Queries;

public record GroupDto(
    Guid Id,
    Guid GroupLeaderId,
    Guid ClientCompanyId,
    Guid ProjectId,
    string Name,
    string Timezone);

public record GetGroupsQuery(Guid? GroupLeaderId = null) : IQuery<IReadOnlyList<GroupDto>>;
