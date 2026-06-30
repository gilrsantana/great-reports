using GreatReports.Domain.Entities;

namespace GreatReports.Application.Common.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<IReadOnlyList<Group>> GetGroupsByPartnerIdAsync(Guid partnerId, CancellationToken cancellationToken = default);
}
