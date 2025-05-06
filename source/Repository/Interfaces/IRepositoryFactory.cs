using WeddingAPI.Repository.Interfaces;

namespace WeddingApi.Repository.Interfaces
{
    public interface IRepositoryFactory<TContext> where TContext : class
    {
        IGenericAsyncRepository<T> GetGenericAsyncRepository<T>() where T : class;
    }
}
