using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.Dtos
{
    public class AsignacionDirigenteCreateDto
    {
        [Required(ErrorMessage = "El ID de usuario es requerido")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El ID de partido político es requerido")]
        public int PartidoPoliticoId { get; set; }
    }
}