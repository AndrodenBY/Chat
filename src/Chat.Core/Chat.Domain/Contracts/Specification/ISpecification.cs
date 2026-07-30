using System.Linq.Expressions;

namespace Chat.Domain.Contracts.Specification;

public interface ISpecification<TValue>
{
    Expression<Func<TValue, bool>>? Criteria { get; }
    Expression<Func<TValue, object>>? OrderBy { get; }
    Expression<Func<TValue, object>>? OrderByDescending { get; }
}
