using GreatReports.Domain.Entities;

namespace GreatReports.Application.Common.Interfaces;

public interface IClientCompanyRepository : IRepository<ClientCompany>
{
    Task<(IReadOnlyList<ClientCompany> Items, int TotalCount)> GetPagedByProviderIdAsync(
        Guid providerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
