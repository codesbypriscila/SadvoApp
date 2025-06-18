using SADVO.Application.Dtos;

namespace SADVO.Application.Interfaces
{
    public interface IAlianzaPoliticaService
    {
        Task<IEnumerable<AlianzaPoliticaDto>> ObtenerSolicitudesPendientesAsync(int partidoId);
        Task<IEnumerable<AlianzaPoliticaDto>> ObtenerSolicitudesEnviadasAsync(int partidoId);
        Task<IEnumerable<AlianzaPoliticaDto>> ObtenerAlianzasActivasAsync(int partidoId);
        Task<AlianzaPoliticaDto?> ObtenerPorIdAsync(int id);
        Task CrearSolicitudAsync(int solicitanteId, int receptorId);
        Task AceptarSolicitudAsync(int id);
        Task RechazarSolicitudAsync(int id);
        Task EliminarSolicitudAsync(int id);
    }
}
