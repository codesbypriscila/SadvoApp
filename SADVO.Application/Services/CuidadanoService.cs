using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Interfaces;

namespace SADVO.Application.Services
{
    public class CiudadanoService : ICiudadanoService
    {
        private readonly IGenericRepository<Ciudadano> _repository;
        private readonly IMapper _mapper;

        public CiudadanoService(IGenericRepository<Ciudadano> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CuidadanoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CuidadanoDto>>(list);
        }

        public async Task<CuidadanoDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<CuidadanoDto>(entity);
        }

        public async Task CreateAsync(CuidadanoDto dto)
        {
            var entity = _mapper.Map<Ciudadano>(dto);
            entity.Activo = true;
            await _repository.AddAsync(entity);
        }

        public Task UpdateAsync(CuidadanoDto dto)
        {
            var entity = _mapper.Map<Ciudadano>(dto);
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
