using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SADVO.Domain.Interfaces;
using SADVO.Infrastructure.AppDbContext;

namespace SADVO.Infrastructure.Repositories
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<Entity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Entity>();
        }

        public async Task<Entity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Entity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<Entity>> FindAsync(Expression<Func<Entity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Entity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void UpdateAsync(Entity entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void RemoveAsync(Entity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<bool> ExistsAsync(Expression<Func<Entity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
