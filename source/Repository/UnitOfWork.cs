using WeddingAPI.Repository.Interfaces;

namespace WeddingAPI.Repository;

public class UnitOfWork<TContext> : IDisposable, IRepositoryFactory<TContext>, IUnitOfWork<TContext> where TContext : DbContext
{
    public TContext _context { get; }
    private readonly ConcurrentDictionary<Type, object> _asyncRepositories;

    public UnitOfWork(TContext context)
    {
        _asyncRepositories = new();
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IGenericAsyncRepository<TEntity> GetGenericAsyncRepository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);

        if (_asyncRepositories[type] is not IAsyncRepository<TEntity> repository)
        {
            throw new Exception("Repository does not implement IGenericAsyncRepository")
        }

        if (!_asyncRepositories.ContainsKey(type))
        {
            _asyncRepositories.TryAdd(type, new GenericAsyncRepository<TEntity>(_context))
        }

        return repository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cToken = null)
    {
        return await _context.SaveChangesAsync(cToken);
    }

    public void Dispose
    {
        Dispose(true);
        GC.SuppressFinalize(true);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _context.Dispose();
        }
    }
}