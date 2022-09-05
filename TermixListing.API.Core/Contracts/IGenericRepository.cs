using TermixListing.API.Core.Models;

namespace TermixListing.API.Core.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<List<TResult>> GetAllAsync<TResult>();
        Task<PageResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
        Task UpdateAsync(T entity);
        Task UpdateAsync<TSource>(int id, TSource source);
        Task<T> AddAsync(T entity);
        Task<TResult> AddAsync<TSource, TResult>(TSource source);
        Task<T> GetAsync(int? id);
        Task<TResult> GetAsync<TResult>(int? id);
        Task DeleteAsync(int id);
        Task<bool> Exist(int id);
    }
}
