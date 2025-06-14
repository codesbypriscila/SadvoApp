using SADVO.Application.Dtos;

namespace SADVO.Application.Interfaces
{
    public interface IPartidoPoliticoService
    {
        Task<IEnumerable<PartidoPoliticoDto>> GetAllAsync();
        Task<PartidoPoliticoDto?> GetByIdAsync(int id);
        Task CreateAsync(PartidoPoliticoDto dto);
        Task UpdateAsync(PartidoPoliticoDto dto);
        Task DeleteAsync(int id);
        Task ActivateAsync(int id);
    }
}