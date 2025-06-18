using System.ComponentModel.DataAnnotations;
using SADVO.Application.Dtos;

namespace SADVO.Application.ViewModels
{
    public class CandidatoPuestoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El candidato es obligatorio.")]
        public int CandidatoId { get; set; }

        [Required(ErrorMessage = "El puesto electivo es obligatorio.")]
        public int PuestoElectivoId { get; set; }

        public int PartidoPoliticoId { get; set; }

        public List<CandidatoDto>? CandidatosDisponibles { get; set; }
        public List<PuestoElectivoDto>? PuestosDisponibles { get; set; }

        public CandidatoPuestoDto ToDto() => new CandidatoPuestoDto
        {
            Id = Id,
            CandidatoId = CandidatoId,
            PuestoElectivoId = PuestoElectivoId,
            PartidoPoliticoId = PartidoPoliticoId
        };
    }

}