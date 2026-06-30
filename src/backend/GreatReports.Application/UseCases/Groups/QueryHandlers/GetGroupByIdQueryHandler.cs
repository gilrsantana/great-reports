using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Groups.Queries;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Groups.QueryHandlers;

public class GetGroupByIdQueryHandler(IGroupRepository groupRepository) : IQueryHandler<GetGroupByIdQuery, GroupDto>
{
    public async Task<Result<GroupDto>> HandleAsync(GetGroupByIdQuery query, CancellationToken cancellationToken = default)
    {
        var group = await groupRepository.GetByIdAsync(query.Id, cancellationToken);
        if (group == null)
        {
            return Result.Failure<GroupDto>(new Error("Group.NotFound", "Grupo não encontrado."));
        }

        var dto = new GroupDto(
            group.Id,
            group.GroupLeaderId,
            group.ClientCompanyId,
            group.ProjectId,
            group.Name,
            group.Timezone
        );

        return Result.Success(dto);
    }
}
