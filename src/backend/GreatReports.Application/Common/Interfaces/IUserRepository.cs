using GreatReports.Domain.Entities;

namespace GreatReports.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByProviderIdAsync(Guid providerId, CancellationToken cancellationToken = default);
    void Delete(User user);
}
