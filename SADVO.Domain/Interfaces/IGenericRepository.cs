using System.Linq.Expressions;

namespace SADVO.Domain.Interfaces
{
    public interface IGenericRepository<Entity> where Entity : class
    {
        Task<Entity?> GetByIdAsync(int id);
        Task<IEnumerable<Entity>> GetAllAsync();
        Task<IEnumerable<Entity>> FindAsync(Expression<Func<Entity, bool>> predicate);
        Task AddAsync(Entity entity);
        void Update(Entity entity);
        void Remove(Entity entity);
        Task<bool> ExistsAsync(Expression<Func<Entity, bool>> predicate);
    }
}
