using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Repositories;
using GAC.WMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GAC.WMS.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        protected readonly GacDbContext _context;
        protected readonly DbSet<T> _entities;

        public Repository(GacDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public async Task<T?> AddAsync(T entity)
        {
            var entry = await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            if (includes != null && includes.Any())
            {
                IQueryable<T> query = _entities;
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                return await query.FirstOrDefaultAsync(e => e.Id == id);
            }
            return await _entities.FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {

            if (includes != null && includes.Any())
            {
                IQueryable<T> query = _entities;
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                return await query.ToListAsync();
            }
            return await _entities.ToListAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _entities.FindAsync(id);
            if (entity != null)
            {
                _entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            var existingEntity = await _entities.FindAsync(entity.Id);

            if (existingEntity == null)
                throw new InvalidOperationException($"Entity with ID {entity.Id} not found.");
            entity.CreatedDate = existingEntity.CreatedDate; // Preserve created date
            entity.CreatedBy = existingEntity.CreatedBy; // Preserve created by user
            entity.UpdatedDate = DateTime.UtcNow; // Update the updated date
            entity.UpdatedBy = existingEntity.UpdatedBy; // Preserve updated by user    
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetWhereAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _entities;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T> GetFirstAsync(
       Expression<Func<T, bool>> predicate,
       params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _entities;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).FirstOrDefaultAsync()!;
        }
    }
}
