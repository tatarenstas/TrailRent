using Microsoft.EntityFrameworkCore;
using TrailRent.Catalog.Dal.Specifications;

namespace TrailRent.Catalog.Dal.Repositories;

public class EfRepository<T>(CatalogDbContext context) : IRepository<T> where T : class
{
    protected CatalogDbContext Context { get; } = context;
    protected DbSet<T> Set => Context.Set<T>();
    public Task AddAsync(T entity, CancellationToken cancellationToken = default) => Set.AddAsync(entity, cancellationToken).AsTask();
    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => Set.FindAsync([id], cancellationToken).AsTask();
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) => await Set.AsNoTracking().ToListAsync(cancellationToken);
    public async Task<IReadOnlyList<T>> ListAsync(Specification<T> specification, CancellationToken cancellationToken = default) => await SpecificationEvaluator.GetQuery(Set.AsNoTracking(), specification).ToListAsync(cancellationToken);
    public Task<int> CountAsync(Specification<T> specification, CancellationToken cancellationToken = default) => SpecificationEvaluator.GetQuery(Set.AsNoTracking(), specification).CountAsync(cancellationToken);
    public void Update(T entity) => Set.Update(entity);
    public void Delete(T entity) => Set.Remove(entity);
}
