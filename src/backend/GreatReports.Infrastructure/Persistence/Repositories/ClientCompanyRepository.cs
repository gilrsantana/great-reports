using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class ClientCompanyRepository : BaseEntityRepository<ClientCompany>, IClientCompanyRepository
{
    public ClientCompanyRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(IReadOnlyList<ClientCompany> Items, int TotalCount)> GetPagedByProviderIdAsync(
        Guid providerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbContext.Set<ClientCompany>()
            .Where(c => c.ProviderCompanyId == providerId && c.Active);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
