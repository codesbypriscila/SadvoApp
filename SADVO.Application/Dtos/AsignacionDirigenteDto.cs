using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.Dtos
{
    public class AsignacionDirigenteDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Username { get; set; } = null!;
        public int PartidoPoliticoId { get; set; }
        public string PartidoPoliticoSiglas { get; set; } = null!;
        public bool Activo { get; set; }
    }

}