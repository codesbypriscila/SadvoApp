using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Dirigente;
using SADVO.Domain.Interfaces;

namespace SADVO.Application.Services
{
    public class DirigenteService : IDirigenteService
    {
        private readonly IGenericRepository<AsignacionDirigente> _repo;

        public DirigenteService(IGenericRepository<AsignacionDirigente> repo)
        {
            _repo = repo;
        }

        public async Task<DirigenteDto?> ObtenerDirigentePorUsuarioIdAsync(int usuarioId)
        {
            var asignaciones = await _repo.FindAsync(a => a.UsuarioId == usuarioId);
            var asignacion = asignaciones.FirstOrDefault();

            if (asignacion == null)
                return null;

            return new DirigenteDto
            {
                UsuarioId = asignacion.UsuarioId,
                PartidoPoliticoId = asignacion.PartidoPoliticoId
            };
        }
    }
}
