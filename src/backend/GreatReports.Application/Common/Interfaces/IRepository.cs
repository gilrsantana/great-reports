using GreatReports.Domain.Entities;

namespace GreatReports.Application.Common.Interfaces;

public interface IRepository<T> : IUnitOfWork where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void UnActivate(T entity);
}
