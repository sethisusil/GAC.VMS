using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Repositories;
using GAC.WMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Infrastructure.Repositories
{
    public class SalesOrderRepository : Repository<SalesOrder>, ISalesOrderRepository
    {
        public SalesOrderRepository(GacDbContext context) : base(context) { }
        public override async Task DeleteAsync(int id)
        {
          
            var entity = await _entities.Include(x=>x.Products).FirstOrDefaultAsync(x=>x.Id==id);
            if (entity != null)
            {
                foreach (var product in entity.Products!)
                {
                    _context.Entry(product).State = EntityState.Deleted;
                }
                _entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
