using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Interfaces;

namespace SADVO.Application.Services
{
    public class AsignacionDirigenteService : IAsignacionDirigenteService
    {
        private readonly IGenericRepository<AsignacionDirigente> _asignacionRepository;
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IGenericRepository<PartidoPolitico> _partidoRepository;
        private readonly IEleccionService _eleccionService;

        public AsignacionDirigenteService(
            IGenericRepository<AsignacionDirigente> asignacionRepository,
            IGenericRepository<Usuario> usuarioRepository,
            IGenericRepository<PartidoPolitico> partidoRepository,
            IEleccionService eleccionService)
        {
            _asignacionRepository = asignacionRepository;
            _usuarioRepository = usuarioRepository;
            _partidoRepository = partidoRepository;
            _eleccionService = eleccionService;
        }

        public async Task<IEnumerable<AsignacionDirigenteDto>> GetAllAsync()
        {
            var asignaciones = await _asignacionRepository.GetAllAsync();
            var usuarios = await _usuarioRepository.GetAllAsync();
            var partidos = await _partidoRepository.GetAllAsync();

            return asignaciones
                .Where(a => a.Activo)
                .Select(a => new AsignacionDirigenteDto
                {
                    Id = a.Id,
                    UsuarioId = a.UsuarioId,
                    Username = usuarios.FirstOrDefault(u => u.Id == a.UsuarioId)?.Username ?? "",
                    PartidoPoliticoId = a.PartidoPoliticoId,
                    PartidoPoliticoSiglas = partidos.FirstOrDefault(p => p.Id == a.PartidoPoliticoId)?.Siglas ?? "",
                    Activo = a.Activo
                });
        }

        public async Task<AsignacionDirigenteDto?> GetByIdAsync(int id)
        {
            var asignacion = await _asignacionRepository.GetByIdAsync(id);
            if (asignacion == null || !asignacion.Activo) return null;

            var usuario = await _usuarioRepository.GetByIdAsync(asignacion.UsuarioId);
            var partido = await _partidoRepository.GetByIdAsync(asignacion.PartidoPoliticoId);

            return new AsignacionDirigenteDto
            {
                Id = asignacion.Id,
                UsuarioId = asignacion.UsuarioId,
                Username = usuario?.Username ?? "",
                PartidoPoliticoId = asignacion.PartidoPoliticoId,
                PartidoPoliticoSiglas = partido?.Siglas ?? "",
                Activo = asignacion.Activo
            };
        }

        public async Task CreateAsync(AsignacionDirigenteCreateDto dto)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                throw new InvalidOperationException("No se pueden crear asignaciones durante una elección activa.");

            if (!await UsuarioEsDirigenteAsync(dto.UsuarioId))
                throw new InvalidOperationException("El usuario no tiene rol de dirigente político.");

            if (await ExisteAsignacionActivaAsync(dto.UsuarioId))
                throw new InvalidOperationException("Este dirigente ya está relacionado con otro partido político.");

            var partido = await _partidoRepository.GetByIdAsync(dto.PartidoPoliticoId);
            if (partido == null || !partido.Activo)
                throw new InvalidOperationException("Partido político no válido o inactivo.");

            var nuevaAsignacion = new AsignacionDirigente
            {
                UsuarioId = dto.UsuarioId,
                PartidoPoliticoId = dto.PartidoPoliticoId,
                Activo = true
            };

            await _asignacionRepository.AddAsync(nuevaAsignacion);
        }

        public async Task DeleteAsync(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                throw new InvalidOperationException("No se pueden eliminar asignaciones durante una elección activa.");

            var asignacion = await _asignacionRepository.GetByIdAsync(id);
            if (asignacion != null)
                _asignacionRepository.RemoveAsync(asignacion);
        }

        public async Task ActivateAsync(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                throw new InvalidOperationException("No se pueden activar asignaciones durante una elección activa.");

            var asignacion = await _asignacionRepository.GetByIdAsync(id);
            if (asignacion != null)
            {
                asignacion.Activo = true;
                _asignacionRepository.UpdateAsync(asignacion);
            }
        }

        public async Task DeactivateAsync(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                throw new InvalidOperationException("No se pueden desactivar asignaciones durante una elección activa.");

            var asignacion = await _asignacionRepository.GetByIdAsync(id);
            if (asignacion != null)
            {
                asignacion.Activo = false;
                _asignacionRepository.UpdateAsync(asignacion);
            }
        }

        public async Task<bool> ExisteAsignacionActivaAsync(int usuarioId)
        {
            return await _asignacionRepository.ExistsAsync(a => a.UsuarioId == usuarioId && a.Activo);
        }

        public async Task<bool> UsuarioEsDirigenteAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            return usuario?.RolId == 2 && usuario.Activo;
        }

        public async Task<bool> HayEleccionActivaAsync()
        {
            return await _eleccionService.HayEleccionActivaAsync();
        }
    }
}
