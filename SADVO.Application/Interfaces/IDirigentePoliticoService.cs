using SADVO.Application.Dtos;

namespace SADVO.Application.Interfaces
{
    public interface IAsignacionDirigenteService
    {
        Task<IEnumerable<AsignacionDirigenteDto>> GetAllAsync();
        Task<AsignacionDirigenteDto?> GetByIdAsync(int id);
        Task CreateAsync(AsignacionDirigenteCreateDto dto);
        Task DeleteAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<bool> ExisteAsignacionActivaAsync(int usuarioId);
        Task<bool> UsuarioEsDirigenteAsync(int usuarioId);
        Task<bool> HayEleccionActivaAsync();
    }

}