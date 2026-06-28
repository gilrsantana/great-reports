using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class BaseEntityRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly GreatReportsDbContext DbContext;

    public BaseEntityRepository(GreatReportsDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>()
            .FirstOrDefaultAsync(e => e.Id == id && e.Active, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>()
            .Where(e => e.Active)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        entity.Update();
        DbContext.Set<TEntity>().Update(entity);
    }

    public virtual void UnActivate(TEntity entity)
    {
        entity.UnActivate();
        DbContext.Set<TEntity>().Update(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.SaveChangesAsync(cancellationToken);
    }
}
