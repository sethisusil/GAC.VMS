using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Repositories;
using GAC.WMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GAC.WMS.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(GacDbContext context) : base(context) { }

        public async Task DeleteAsync(string code)
        {
            var entity = await GetByCodeAsync(code);
            if (entity != null)
            {
                _entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Product?> GetByCodeAsync(string code, params Expression<Func<Product, object>>[] includes)
        {
            if (includes != null && includes.Any())
            {
                IQueryable<Product> query = _entities;
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                return await query.FirstOrDefaultAsync(p => p.Code == code);
            }
            return await _context.Products.FirstOrDefaultAsync(p => p.Code == code);
        }
    }
}
