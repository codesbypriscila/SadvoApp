using SADVO.Domain.Entities;

namespace SADVO.Application.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> GetRolesDisponiblesAsync();
        Task<bool> RolExisteAsync(int rolId);
    }
}