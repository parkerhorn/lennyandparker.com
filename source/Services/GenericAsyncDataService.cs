using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using WeddingAPI.Repository.Interfaces;
using WeddingAPI.Services.Interfaces;

namespace WeddingAPI.Services;

public class GenericAsyncDataService<TEntity, TContext> : IGenericAsyncDataService<TEntity, TContext>
    where TEntity : class
    where TContext : DbContext
{
    protected readonly IUnitOfWork<TContext> _unitOfWork;
    protected readonly ILogger<GenericAsyncDataService<TEntity, TContext>> _logger;

    public GenericAsyncDataService(IUnitOfWork<TContext> unitOfWork, ILogger<GenericAsyncDataService<TEntity, TContext>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await _unitOfWork.GetGenericAsyncRepository<TEntity>().GetListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to retrieve all entities for type {typeof(TEntity)}. Message: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<TEntity> GetByIdAsync(object id)
    {
        try
        {
            return await _unitOfWork.GetGenericAsyncRepository<TEntity>().GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to retrieve entity for ID: {id}. Message: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await _unitOfWork.GetGenericAsyncRepository<TEntity>().SingleTransactionAsync(predicate);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to retrieve entity. Message: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        try
        {
            var repo = _unitOfWork.GetGenericAsyncRepository<TEntity>();
            await repo.AddAsync(entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to add entity. Message: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<IList<TEntity>> AddAsync(IList<TEntity> entities)
    {
        try
        {
            await _unitOfWork.GetGenericAsyncRepository<TEntity>().AddAsync(entities);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to add entities. Message: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<TEntity> AddAndSaveAsync(TEntity entity)
    {
        try
        {
            await _unitOfWork.GetGenericAsyncRepository<TEntity>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(new CancellationToken());
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to add and save entity. Message: {ex.Message}");
            throw;
        }
    }

    public virtual TEntity Update(TEntity entity)
    {
        try
        {
            _unitOfWork.GetGenericAsyncRepository<TEntity>().Update(entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to update entity. Message: {ex.Message}");
            throw;
        }
    }

    public virtual IList<TEntity> Update(IList<TEntity> entities)
    {
        try
        {
            _unitOfWork.GetGenericAsyncRepository<TEntity>().Update(entities);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to update entities. Message: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<TEntity> UpdateAndSaveAsync(TEntity entity)
    {
        try
        {
            _unitOfWork.GetGenericAsyncRepository<TEntity>().Update(entity);
            await _unitOfWork.SaveChangesAsync(new CancellationToken());
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to update and save entity. Message: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<IList<TEntity>> UpdateAndSaveAsync(IList<TEntity> entities)
    {
        try
        {
            _unitOfWork.GetGenericAsyncRepository<TEntity>().Update(entities);
            await _unitOfWork.SaveChangesAsync(new CancellationToken());
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to update and save entities. Message: {ex.Message}");
            throw;
        }
    }

    public virtual void Delete(TEntity entity)
    {
        try
        {
            _unitOfWork.GetGenericAsyncRepository<TEntity>().Delete(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error encountered when deleting entity. Message: {ex.Message}");
            throw;
        }
    }

    public virtual void Delete(IList<TEntity> entities)
    {
        try
        {
            _unitOfWork.GetGenericAsyncRepository<TEntity>().Delete(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error encountered when deleting entities. Message: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<TEntity> DeleteAndSaveAsync(TEntity entity)
    {
        try
        {
            _unitOfWork.GetGenericAsyncRepository<TEntity>().Delete(entity);
            await _unitOfWork.SaveChangesAsync(new CancellationToken());
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to delete and save entity. Message: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<IList<TEntity>> DeleteAndSaveAsync(IList<TEntity> entities)
    {
        try
        {
            _unitOfWork.GetGenericAsyncRepository<TEntity>().Delete(entities);
            await _unitOfWork.SaveChangesAsync(new CancellationToken());
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to delete and save entities. Message: {ex.Message}");
            throw;
        }
    }

    public EntityState GetContextState<TEntityUser>(TEntity entity)
    {
        try
        {
            return _unitOfWork._context.Entry(entity).State;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to get context state. Message: {ex.Message}");
            throw;
        }
    }

    public EntityState SetContextState<TEntityUser>(TEntity entity, EntityState state)
    {
        try
        {
            _unitOfWork._context.Entry(entity).State = state;
            return state;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to set context state. Message: {ex.Message}");
            throw;
        }
    }

    public Task SaveChangesAsync()
    {
        try
        {
            return _unitOfWork.SaveChangesAsync(new CancellationToken());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to save changes. Message: {ex.Message}");
            throw;
        }
    }
}