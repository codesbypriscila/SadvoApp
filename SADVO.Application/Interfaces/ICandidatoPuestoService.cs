using SADVO.Application.Dtos;

namespace SADVO.Application.Interfaces
{
    public interface ICandidatoPuestoService
    {
        Task<IEnumerable<CandidatoPuestoDto>> GetAllAsync(int partidoId);
        Task CreateAsync(CandidatoPuestoDto dto);
        Task DeleteAsync(int id);
        Task<List<CandidatoDto>> ObtenerCandidatosDisponiblesAsync(int partidoId, int puestoId);
        Task<List<PuestoElectivoDto>> ObtenerPuestosDisponiblesAsync(int partidoId);
    }

}