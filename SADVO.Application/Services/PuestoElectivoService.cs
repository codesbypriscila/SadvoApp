using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Interfaces;

namespace SADVO.Application.Services
{
    public class PuestoElectivoService : IPuestoElectivoService
    {
        private readonly IGenericRepository<PuestoElectivo> _repository;
        private readonly IMapper _mapper;

        public PuestoElectivoService(IGenericRepository<PuestoElectivo> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PuestoElectivoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PuestoElectivoDto>>(list);
        }

        public async Task<PuestoElectivoDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<PuestoElectivoDto>(entity);
        }

        public async Task CreateAsync(PuestoElectivoDto dto)
        {
            var entity = _mapper.Map<PuestoElectivo>(dto);
            entity.Activo = true;
            await _repository.AddAsync(entity);
        }

        public Task UpdateAsync(PuestoElectivoDto dto)
        {
            var entity = _mapper.Map<PuestoElectivo>(dto);
            _repository.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is not null)
            {
                entity.Activo = false;
                _repository.Update(entity);
            }
        }

        public async Task ActivateAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is not null)
            {
                entity.Activo = true;
                _repository.Update(entity);
            }
        }
    }
}
