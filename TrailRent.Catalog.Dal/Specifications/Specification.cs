using System.Linq.Expressions;

namespace TrailRent.Catalog.Dal.Specifications;

public abstract class Specification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; protected init; }
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public Expression<Func<T, object>>? OrderBy { get; protected init; }
    public bool Descending { get; protected init; }
    public int? Skip { get; protected init; }
    public int? Take { get; protected init; }
}
