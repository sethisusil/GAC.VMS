using GAC.WMS.Domain.Entities;
using System.Linq.Expressions;

namespace GAC.WMS.Domain.Repositories
{
    public interface IRepository<T> where T: Entity
    {
        Task<T?> AddAsync(T customer);
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetWhereAsync(
         Expression<Func<T, bool>> predicate,
         params Expression<Func<T, object>>[] includes);
        Task<T> GetFirstAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task DeleteAsync(int id);
        Task UpdateAsync(T customer);
    }
}
