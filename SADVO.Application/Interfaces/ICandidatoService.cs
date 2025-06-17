using SADVO.Application.Dtos;

namespace SADVO.Application.Interfaces
{
    public interface ICandidatoService
    {
        Task<IEnumerable<CandidatoDto>> GetAllAsync();
        Task<CandidatoDto?> GetByIdAsync(int id);
        Task CreateAsync(CandidatoDto dto);
        Task UpdateAsync(CandidatoDto dto);
        Task DeleteAsync(int id);
        Task ActivateAsync(int id);
    }
}
