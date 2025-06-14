using GAC.WMS.Domain.Entities;
using System.Linq.Expressions;

namespace GAC.WMS.Domain.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByCodeAsync(string code, params Expression<Func<Product, object>>[] includes);
        Task DeleteAsync(string code);
    }
}
