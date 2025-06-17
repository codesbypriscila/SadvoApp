using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.ViewModels
{
    public class AsignacionDirigenteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un dirigente político.")]
        [Display(Name = "Dirigente Político")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un partido político.")]
        [Display(Name = "Partido Político")]
        public int PartidoPoliticoId { get; set; }

        public string? Username { get; set; }
        public string? PartidoPoliticoSiglas { get; set; }

        public bool Activo { get; set; }
    }
}
