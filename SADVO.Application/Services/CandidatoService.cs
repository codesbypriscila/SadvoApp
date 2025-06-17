using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Dirigente;
using SADVO.Domain.Interfaces;

namespace SADVO.Application.Services
{
    public class CandidatoService : ICandidatoService
    {
        private readonly IGenericRepository<Candidato> _repository;
        private readonly IMapper _mapper;

        public CandidatoService(IGenericRepository<Candidato> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CandidatoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CandidatoDto>>(list);
        }

        public async Task<CandidatoDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<CandidatoDto>(entity);
        }

        public async Task CreateAsync(CandidatoDto dto)
        {
            var entity = _mapper.Map<Candidato>(dto);
            entity.Activo = true;
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(CandidatoDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null) throw new InvalidOperationException("Candidato no encontrado.");

            entity.Nombre = dto.Nombre;
            entity.Apellido = dto.Apellido;
            entity.FotoUrl = dto.FotoUrl;

            _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Activo = false;
                _repository.UpdateAsync(entity);
            }
        }

        public async Task ActivateAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Activo = true;
                _repository.UpdateAsync(entity);
            }
        }
    }
}
