using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using WeddingAPI.Repository.Interfaces;

namespace WeddingAPI.Repository;

public class UnitOfWork<TContext> : IDisposable, IRepositoryFactory<TContext>, IUnitOfWork<TContext> where TContext : DbContext
{
    public TContext _context { get; }
    private readonly ConcurrentDictionary<Type, object> _asyncRepositories;
    private bool disposedValue;

    public UnitOfWork(TContext context)
    {
        _asyncRepositories = new();
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IGenericAsyncRepository<TEntity> GetGenericAsyncRepository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);

        if (!_asyncRepositories.ContainsKey(type))
        {
            _asyncRepositories.TryAdd(type, new GenericAsyncRepository<TEntity>(_context));
        }

        if (_asyncRepositories[type] is not IGenericAsyncRepository<TEntity> repository)
        {
            throw new Exception("Repository does not implement IGenericAsyncRepository");
        }

        return repository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cToken)
    {
        return await _context.SaveChangesAsync(cToken);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}