using System.ComponentModel.DataAnnotations;
using SADVO.Application.Dtos;

namespace SADVO.Application.ViewModels
{
    public class EleccionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la elección es obligatorio")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de realización es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaRealizacion { get; set; }

        public bool Activa { get; set; }

        public static EleccionViewModel FromDto(EleccionDto dto)
        {
            return new EleccionViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                FechaRealizacion = dto.FechaRealizacion,
                Activa = dto.Activa
            };
        }

        public EleccionDto ToDto()
        {
            return new EleccionDto
            {
                Id = this.Id,
                Nombre = this.Nombre,
                FechaRealizacion = this.FechaRealizacion,
                Activa = this.Activa
            };
        }
    }
}
