using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Interfaces;

namespace SADVO.Application.Services
{
    public class EleccionService : IEleccionService
    {
        private readonly IGenericRepository<Eleccion> _repository;

        public EleccionService(IGenericRepository<Eleccion> repository)
        {
            _repository = repository;
        }

        public async Task<bool> HayEleccionActivaAsync()
        {
            var elecciones = await _repository.GetAllAsync();
            return elecciones.Any(e => e.Activa);
        }
    }
}
