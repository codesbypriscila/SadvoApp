using SADVO.Application.Dtos;

namespace SADVO.Application.Interfaces
{
    public interface IEleccionService
    {
        Task<bool> HayEleccionActivaAsync();
        Task<IEnumerable<EleccionDto>> GetAllAsync();
        Task<EleccionDto?> GetByIdAsync(int id);
        Task CreateAsync(EleccionDto dto);
        Task UpdateAsync(EleccionDto dto);
        Task FinalizarAsync(int id);
        Task<ResultadoEleccionDto?> ObtenerResultadosAsync(int eleccionId);
    }
}
