using Chat.Domain.Common.Pagination;
using Chat.Domain.Contracts.Specifications;

namespace Chat.Domain.Contracts;

public interface IRepository<TEntity, in TId>
{
    Task<TEntity?> GetById(TId id, CancellationToken cancellationToken);
    Task<TEntity?> GetBySpecification(ISpecification<TEntity> specification, CancellationToken cancellationToken);
    Task<PaginatedList<TEntity>> GetAll(ISpecification<TEntity> specification, PaginationParameters parameters, CancellationToken cancellationToken);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
