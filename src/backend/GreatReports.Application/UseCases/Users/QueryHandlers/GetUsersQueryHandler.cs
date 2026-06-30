using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Users.Queries;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Users.QueryHandlers;

public class GetUsersQueryHandler(
    IUserRepository userRepository,
    IIdentityService identityService) : IQueryHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<Result<IReadOnlyList<UserDto>>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetByProviderIdAsync(query.ProviderId, cancellationToken);
        var userIds = users.Select(u => u.Id).ToList();

        var rolesMap = await identityService.GetUserRolesAsync(userIds);

        var dtos = users.Select(u => new UserDto(
            u.Id,
            u.DisplayName,
            u.Email,
            rolesMap.TryGetValue(u.Id, out var role) ? role : "Partner",
            u.EmailConfirmed
        )).ToList();

        return Result<IReadOnlyList<UserDto>>.Success(dtos);
    }
}
