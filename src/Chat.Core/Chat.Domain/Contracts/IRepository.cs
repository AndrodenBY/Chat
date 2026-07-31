using Chat.Domain.Common.Pagination;
using Chat.Domain.Contracts.Specification;

namespace Chat.Domain.Contracts;

public interface IRepository<TEntity, in TId>
{
    Task<TEntity?> GetById(TId id, CancellationToken cancellationToken);
    Task<TEntity?> GetBySpecification(ISpecification<TEntity> specification, CancellationToken cancellationToken);
    Task<PaginatedList<TEntity>> GetAll(ISpecification<TEntity> specification, PaginationParameters parameters, CancellationToken cancellationToken);
    Task<TEntity> Add(TEntity entity, CancellationToken cancellationToken);
    Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken);
    Task<bool> Remove(TEntity entity, CancellationToken cancellationToken);
}
