using System.Linq.Expressions;

namespace CNPM.Infrastructure.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
}
