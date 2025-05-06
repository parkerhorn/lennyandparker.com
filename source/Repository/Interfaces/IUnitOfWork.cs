using Microsoft.EntityFrameworkCore;

namespace WeddingAPI.Repository.Interfaces;

public interface IUnitOfWork<out TContext> : IDisposable where TContext : DbContext
{
    public TContext _context { get; }

    public IGenericAsyncRepository<TEntity> GetGenericAsyncRepository<TEntity>() where TEntity : class;

    public Task<int> SaveChangesAsync(CancellationToken cToken);
}