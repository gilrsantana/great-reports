using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Groups.Queries;

public record GetGroupByIdQuery(Guid Id) : IQuery<GroupDto>;
