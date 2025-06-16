using SADVO.Application.Interfaces;
using SADVO.Domain.Entities;
using SADVO.Domain.Interfaces;

namespace SADVO.Application.Services
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _repository;

        public RolService(IGenericRepository<Rol> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Rol>> GetRolesDisponiblesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> RolExisteAsync(int rolId)
        {
            var roles = await _repository.GetAllAsync();
            return roles.Any(r => r.Id == rolId);
        }

    }
}