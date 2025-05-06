namespace WeddingAPI.Repository.Interfaces;

public interface IUnitOfWork<out TContext> : IDisposable where TContext : DbContext
{
    public TContext Context { get; }

    public IGenericAsyncRepository<TEntity> GetGenericAsyncRepository<TEntity>() where TEntity : class

    public Task<int> SaveChangesAsync(CancellationToken cToken = null)
}