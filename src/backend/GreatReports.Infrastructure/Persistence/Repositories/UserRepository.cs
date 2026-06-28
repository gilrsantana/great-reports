using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class UserRepository : BaseEntityRepository<User>, IUserRepository
{
    public UserRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email && u.Active, cancellationToken);
    }

    public void Delete(User user)
    {
        DbContext.Set<User>().Remove(user);
    }
}
