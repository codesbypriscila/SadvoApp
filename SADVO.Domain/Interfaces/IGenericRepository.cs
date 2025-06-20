using System.Linq.Expressions;
using SADVO.Domain.Entities.Administrador;

namespace SADVO.Domain.Interfaces
{
    public interface IGenericRepository<Entity> where Entity : class
    {
        Task<Entity?> GetByIdAsync(int id);
        Task<IEnumerable<Entity>> GetAllAsync();
        Task<IEnumerable<Entity>> FindAsync(Expression<Func<Entity, bool>> predicate, Func<IQueryable<Entity>, IQueryable<Entity>>? include = null);
        Task AddAsync(Entity entity);
        void UpdateAsync(Entity entity);
        void RemoveAsync(Entity entity);
        Task<bool> ExistsAsync(Expression<Func<Entity, bool>> predicate);
        Task<IEnumerable<Eleccion>> GetAllIncludingAsync(params Expression<Func<Eleccion, object>>[] includes);
        Task<Entity?> GetFirstOrDefaultAsync(Expression<Func<Entity, bool>> predicate, Func<IQueryable<Entity>, IQueryable<Entity>>? include = null);


    }
}
