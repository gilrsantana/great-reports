using GreatReports.Domain.Entities;

namespace GreatReports.Application.Common.Interfaces;

public interface IScheduledEmailRepository : IRepository<ScheduledEmail>
{
    Task<IReadOnlyList<ScheduledEmail>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
}
