using System.Linq.Expressions;

namespace Chat.Domain.Contracts.Specifications;

public abstract class BaseSpecification<TValue>(Expression<Func<TValue, bool>>? criteria = null) 
    : ISpecification<TValue>
{
    public Expression<Func<TValue, bool>>? Criteria { get; } = criteria;
    public Expression<Func<TValue, object>>? OrderBy { get; private set; }
    public Expression<Func<TValue, object>>? OrderByDescending { get; private set; }

    protected void AddOrderBy(Expression<Func<TValue, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void AddOrderByDescending(Expression<Func<TValue, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }
}
