using SADVO.Application.Dtos;

namespace SADVO.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto>> GetAllAsync();
        Task<UsuarioDto?> GetByIdAsync(int id);
        Task CreateAsync(UsuarioDto dto, string password);
        Task UpdateAsync(int id, UsuarioDto dto, string? newPassword);
        Task ActivateAsync(int id);
        Task DeleteAsync(int id);
        Task<bool> IsUsernameTakenAsync(string username, int? excludeUserId = null);
    }
}