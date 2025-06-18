using SADVO.Application.Dtos;

public interface IDirigenteService
{
    Task<DirigenteDto?> ObtenerDirigentePorUsuarioIdAsync(int usuarioId);
}
