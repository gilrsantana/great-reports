using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class ProviderCompanyRepository : BaseEntityRepository<ProviderCompany>, IProviderCompanyRepository
{
    public ProviderCompanyRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<ProviderCompany>()
            .AnyAsync(p => p.TaxId == taxId && p.Active, cancellationToken);
    }
}
