using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using WeddingAPI.Repository.Interfaces;

namespace WeddingAPI.Repository;

public class GenericAsyncRepository<T> : IGenericAsyncRepository<T> where T : class
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public GenericAsyncRepository(DbContext context)
    {
        _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<T> SingleTransactionAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool disableTracking = false
    )
    {
        IQueryable<T> query = _dbSet;

        if (disableTracking) query = query.AsNoTracking();
        if (include != null) query = include(query);
        if (predicate != null) query = query.Where(predicate);

        return orderBy != null ? await orderBy(query).FirstOrDefaultAsync() : await query.FirstOrDefaultAsync();
    }

    public async Task<IList<T>> GetListAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cToken = default,
        bool disableTracking = false,
        bool distinct = false
    )
    {
        IQueryable<T> query = _dbSet;

        if (disableTracking) query = query.AsNoTracking();
        if (include != null) query = include(query);
        if (predicate != null) query = query.Where(predicate);
        if (distinct) query = query.Distinct();

        return await (orderBy != null ? orderBy(query).ToListAsync(cToken) : query.ToListAsync(cToken));
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken cToken = default)
    {
        return _dbSet.AddAsync(entity, cToken);
    }

    public Task AddAsync(params T[] entities)
    {
        return _dbSet.AddRangeAsync(entities);
    }

    public Task AddAsync(IEnumerable<T> entities, CancellationToken cToken = default)
    {
        return _dbSet.AddRangeAsync(entities, cToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Update(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public async Task DeleteAsync(object id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void Delete(IList<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}