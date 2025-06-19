using Microsoft.AspNetCore.Http;
using SADVO.Application.Dtos;
using SADVO.Application.ViewModels;

namespace SADVO.Application.Interfaces
{
    public interface IVotoService
    {
        Task<string> ValidarCedulaInicialAsync(string cedula);
        Task<string> ProcesarOCRAsync(IFormFile imagen);
        Task<List<SeleccionCandidatoViewModel>> ObtenerPuestosYOpcionesAsync(int ciudadanoId);
        Task<string> RegistrarVotoAsync(List<VotoDto> votos, int ciudadanoId);
        Task<bool> YaHaVotadoAsync(int ciudadanoId, int eleccionId);
    }
}
