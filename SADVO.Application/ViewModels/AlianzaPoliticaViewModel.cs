using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SADVO.Application.ViewModels
{
    public class AlianzaPoliticaViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un partido receptor")]
        [Display(Name = "Partido Receptor")]
        public int PartidoReceptorId { get; set; }

        public List<SelectListItem> PartidosDisponibles { get; set; } = new();

        public string? PartidoReceptorNombre { get; set; }
        public string? PartidoReceptorSiglas { get; set; }

        public int? IdSolicitud { get; set; } 
    }
}
