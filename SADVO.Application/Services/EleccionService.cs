using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Interfaces;
using AutoMapper;

namespace SADVO.Application.Services
{
    public class EleccionService : IEleccionService
    {
        private readonly IGenericRepository<Eleccion> _repository;
        private readonly IMapper _mapper;

        public EleccionService(IGenericRepository<Eleccion> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> HayEleccionActivaAsync()
        {
            var elecciones = await _repository.GetAllAsync();
            return elecciones.Any(e => e.Activa);
        }

        public async Task<IEnumerable<EleccionDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EleccionDto>>(list);
        }

        public async Task<EleccionDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<EleccionDto>(entity) : null;
        }

        public async Task CreateAsync(EleccionDto dto)
        {
            var entity = _mapper.Map<Eleccion>(dto);
            await _repository.AddAsync(entity);
        }

        public Task UpdateAsync(EleccionDto dto)
        {
            var entity = _mapper.Map<Eleccion>(dto);
            _repository.UpdateAsync(entity);
            return Task.CompletedTask;
        }

        public async Task FinalizarAsync(int id)
        {
            var eleccion = await _repository.GetByIdAsync(id);
            if (eleccion != null && eleccion.Activa)
            {
                eleccion.Activa = false;
                _repository.UpdateAsync(eleccion);

            }
        }
    }
}
