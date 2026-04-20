using System.Linq.Expressions;
using CNPM.Models;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly PharmacyDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(PharmacyDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> Query() => _dbSet.AsQueryable();

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => _dbSet.AnyAsync(predicate, cancellationToken);

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => _dbSet.AddAsync(entity, cancellationToken).AsTask();

    public void AddRange(IEnumerable<T> entities)
        => _dbSet.AddRange(entities);

    public void Update(T entity)
        => _dbSet.Update(entity);
}
