using SADVO.Application.Dtos;

namespace SADVO.Application.Interfaces
{
    public interface ICiudadanoService
    {
        Task<IEnumerable<CuidadanoDto>> GetAllAsync();
        Task<CuidadanoDto?> GetByIdAsync(int id);
        Task CreateAsync(CuidadanoDto dto);
        Task UpdateAsync(CuidadanoDto dto);
        Task DeleteAsync(int id); 
        Task ActivateAsync(int id); 
    }
}
