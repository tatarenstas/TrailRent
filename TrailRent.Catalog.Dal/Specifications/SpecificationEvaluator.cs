using Microsoft.EntityFrameworkCore;

namespace TrailRent.Catalog.Dal.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(IQueryable<T> query, Specification<T> specification) where T : class
    {
        if (specification.Criteria is not null) query = query.Where(specification.Criteria);
        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
        if (specification.OrderBy is not null)
            query = specification.Descending ? query.OrderByDescending(specification.OrderBy) : query.OrderBy(specification.OrderBy);
        if (specification.Skip is not null) query = query.Skip(specification.Skip.Value);
        if (specification.Take is not null) query = query.Take(specification.Take.Value);
        return query;
    }
}
