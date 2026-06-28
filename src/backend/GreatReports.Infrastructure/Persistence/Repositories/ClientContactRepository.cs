using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class ClientContactRepository : BaseEntityRepository<ClientContact>, IClientContactRepository
{
    public ClientContactRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ClientContact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<ClientContact>()
            .FirstOrDefaultAsync(c => c.Email == email && c.Active, cancellationToken);
    }
}
