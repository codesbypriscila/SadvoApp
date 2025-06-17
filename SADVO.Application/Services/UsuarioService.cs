using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Interfaces;
using SADVO.Application.Utils;

namespace SADVO.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repository;
        private readonly IGenericRepository<AsignacionDirigente> _asignacionRepository;
        private readonly IMapper _mapper;
        private readonly IRolService _rolService;

        public UsuarioService(
            IGenericRepository<Usuario> repository,
            IMapper mapper,
            IRolService rolService,
            IGenericRepository<AsignacionDirigente> asignacionRepository)

        {
            _repository = repository;
            _mapper = mapper;
            _rolService = rolService;
            _asignacionRepository = asignacionRepository;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
        {
            var usuarios = await _repository.GetAllAsync();
            return usuarios.Select(u =>
            {
                var dto = _mapper.Map<UsuarioDto>(u);
                dto.Rol = GetNombreRol(u.RolId);
                return dto;
            });
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return null;

            var dto = _mapper.Map<UsuarioDto>(usuario);
            dto.Rol = GetNombreRol(usuario.RolId);
            return dto;
        }

        private string GetNombreRol(int rolId)
        {
            return rolId switch
            {
                1 => "Administrador",
                2 => "Dirigente",
                _ => "Sin Rol"
            };
        }

        public async Task CreateAsync(UsuarioDto dto, string password)
        {
            if (!await _rolService.RolExisteAsync(dto.RolId))
            {
                throw new InvalidOperationException("El rol especificado no existe.");
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Username = dto.Username,
                RolId = dto.RolId,
                Activo = true,
                PasswordHash = PasswordHasher.Hash(password)
            };

            await _repository.AddAsync(usuario);
        }

        public async Task UpdateAsync(int id, UsuarioDto dto, string? newPassword)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return;

            if (!await _rolService.RolExisteAsync(dto.RolId))
            {
                throw new InvalidOperationException("El rol especificado no existe.");
            }

            entity.Nombre = dto.Nombre;
            entity.Apellido = dto.Apellido;
            entity.Email = dto.Email;
            entity.Username = dto.Username;
            entity.RolId = dto.RolId;
            entity.Activo = dto.Activo;

            if (!string.IsNullOrWhiteSpace(newPassword))
                entity.PasswordHash = PasswordHasher.Hash(newPassword);

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

        public async Task<bool> IsUsernameTakenAsync(string username, int? excludeUserId = null)
        {
            var allUsers = await _repository.GetAllAsync();
            return allUsers.Any(u =>
                u.Username.ToLower() == username.ToLower() &&
                (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }

        public async Task<IEnumerable<UsuarioDto>> GetUsuariosDirigentesDisponiblesAsync()
        {
            var usuarios = await _repository.GetAllAsync();
            var asignaciones = await _asignacionRepository.GetAllAsync();

            var disponibles = usuarios
                .Where(u => u.RolId == 2 && u.Activo && !asignaciones.Any(a => a.UsuarioId == u.Id && a.Activo))
                .Select(u =>
                {
                    var dto = _mapper.Map<UsuarioDto>(u);
                    dto.Rol = GetNombreRol(u.RolId);
                    return dto;
                });

            return disponibles;
        }

    }
}