using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Interfaces;

namespace SADVO.Application.Services
{
    public class PartidoPoliticoService : IPartidoPoliticoService
    {
        private readonly IGenericRepository<PartidoPolitico> _repository;
        private readonly IMapper _mapper;

        public PartidoPoliticoService(IGenericRepository<PartidoPolitico> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PartidoPoliticoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PartidoPoliticoDto>>(list);
        }

        public async Task<PartidoPoliticoDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<PartidoPoliticoDto>(entity);
        }

        public async Task CreateAsync(PartidoPoliticoDto dto)
        {
            var entity = _mapper.Map<PartidoPolitico>(dto);
            entity.Activo = true;
            await _repository.AddAsync(entity);
        }

        public Task UpdateAsync(PartidoPoliticoDto dto)
        {
            var entity = _mapper.Map<PartidoPolitico>(dto);
            _repository.UpdateAsync(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is not null)
            {
                entity.Activo = false;
                _repository.UpdateAsync(entity);
            }
        }

        public async Task ActivateAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is not null)
            {
                entity.Activo = true;
                _repository.UpdateAsync(entity);
            }
        }
    }
}
