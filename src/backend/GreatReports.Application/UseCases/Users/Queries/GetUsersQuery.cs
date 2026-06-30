using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.Users.Queries;

public record UserDto(Guid Id, string DisplayName, string Email, string Role, bool EmailConfirmed);

public record GetUsersQuery(Guid ProviderId) : IQuery<IReadOnlyList<UserDto>>;
