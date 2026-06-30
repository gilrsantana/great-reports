using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Groups.Queries;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Groups.QueryHandlers;

public class GetGroupsQueryHandler(IGroupRepository groupRepository) : IQueryHandler<GetGroupsQuery, IReadOnlyList<GroupDto>>
{
    public async Task<Result<IReadOnlyList<GroupDto>>> HandleAsync(GetGroupsQuery query, CancellationToken cancellationToken = default)
    {
        var allGroups = await groupRepository.GetAllAsync(cancellationToken);
        
        var filtered = allGroups.AsEnumerable();
        if (query.GroupLeaderId.HasValue)
        {
            filtered = filtered.Where(g => g.GroupLeaderId == query.GroupLeaderId.Value);
        }

        var dtos = filtered.Select(g => new GroupDto(
            g.Id,
            g.GroupLeaderId,
            g.ClientCompanyId,
            g.ProjectId,
            g.Name,
            g.Timezone
        )).ToList();

        return Result.Success<IReadOnlyList<GroupDto>>(dtos);
    }
}
