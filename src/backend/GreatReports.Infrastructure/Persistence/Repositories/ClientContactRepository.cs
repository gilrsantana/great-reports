using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class ClientContactRepository(GreatReportsDbContext dbContext) : BaseEntityRepository<ClientContact>(dbContext), IClientContactRepository
{
    public Task<ClientContact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<ClientContact>()
            .FirstOrDefaultAsync(c => c.Email == email && c.Active, cancellationToken);
    }

    public async Task<IReadOnlyList<ClientContact>> GetByClientCompanyIdAsync(Guid clientCompanyId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<ClientContact>()
            .Where(c => c.ClientCompanyId == clientCompanyId && c.Active)
            .ToListAsync(cancellationToken);
    }

    public void Delete(ClientContact contact)
    {
        DbContext.Set<ClientContact>().Remove(contact);
    }
}
