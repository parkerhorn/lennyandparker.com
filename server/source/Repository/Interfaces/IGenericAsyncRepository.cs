using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace WeddingApi.Repository.Interfaces;

public interface IGenericAsyncRepository<T> where T : class
{
    public Task<T> SingleTransactionAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool disableTracking = false
        );

    public Task<IList<T>> GetListAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cToken = default,
        bool disableTracking = false,
        bool distinct = false
        );

    public Task<T?> GetByIdAsync(object id, object value);

    public ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken cToken = default);

    public Task AddAsync(params T[] entities);

    public Task AddAsync(IEnumerable<T> entities, CancellationToken cToken = default);

    public void Update(T entity);

    public void Update(IEnumerable<T> entities);

    public Task DeleteAsync(object id);

    public void Delete(T entity);

    public void Delete(IList<T> entities);
}