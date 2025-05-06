using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WeddingAPI.Services.Interfaces;

public interface IGenericAsyncDataService<TEntity, TContext> where TEntity : class where TContext : DbContext
{
    public Task<IEnumerable<TEntity>> GetAllAsync();

    public Task<TEntity> GetByIdAsync(object id);

    public Task<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    public Task<TEntity> AddAsync(TEntity entity);

    public Task<IList<TEntity>> AddAsync(IList<TEntity> entities);

    public Task<TEntity> AddAndSaveAsync(TEntity entity);

    public TEntity Update(TEntity entity);

    public IList<TEntity> Update(IList<TEntity> entities);

    public Task<TEntity> UpdateAndSaveAsync(TEntity entity);

    public Task<IList<TEntity>> UpdateAndSaveAsync(IList<TEntity> entities);

    public void Delete(TEntity entity);

    public void Delete(IList<TEntity> entities);

    public Task<TEntity> DeleteAndSaveAsync(TEntity entity);

    public Task<IList<TEntity>> DeleteAndSaveAsync(IList<TEntity> entities);

    public EntityState GetContextState<TEntityUser>(TEntity entity);

    public EntityState SetContextState<TEntityUser>(TEntity entity, EntityState state);

    public Task SaveChangesAsync();
}