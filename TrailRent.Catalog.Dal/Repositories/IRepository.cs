using TrailRent.Catalog.Dal.Specifications;

namespace TrailRent.Catalog.Dal.Repositories;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
