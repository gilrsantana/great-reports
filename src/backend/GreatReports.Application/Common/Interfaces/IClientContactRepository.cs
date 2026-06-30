using GreatReports.Domain.Entities;

namespace GreatReports.Application.Common.Interfaces;

public interface IClientContactRepository : IRepository<ClientContact>
{
    Task<ClientContact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClientContact>> GetByClientCompanyIdAsync(Guid clientCompanyId, CancellationToken cancellationToken = default);
    void Delete(ClientContact contact);
}
