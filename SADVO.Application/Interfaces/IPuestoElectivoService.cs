using SADVO.Application.Dtos;

namespace SADVO.Application.Interfaces
{
    public interface IPuestoElectivoService
    {
        Task<IEnumerable<PuestoElectivoDto>> GetAllAsync();
        Task<PuestoElectivoDto?> GetByIdAsync(int id);
        Task CreateAsync(PuestoElectivoDto dto);
        Task UpdateAsync(PuestoElectivoDto dto);
        Task DeleteAsync(int id); 
        Task ActivateAsync(int id); 
    }
}
