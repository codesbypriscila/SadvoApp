using SADVO.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.ViewModels
{
    public class PuestoElectivoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del puesto es obligatorio.")]
        [StringLength(20, ErrorMessage = "El nombre no debe exceder los 20 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(200, ErrorMessage = "La descripción no debe exceder los 200 caracteres.")]
        public string Descripcion { get; set; } = null!;

        public bool Activo { get; set; }

        public PuestoElectivoDto ToDto() =>
            new PuestoElectivoDto
            {
                Id = Id,
                Nombre = Nombre,
                Descripcion = Descripcion,
                Activo = Activo
            };

        public static PuestoElectivoViewModel FromDto(PuestoElectivoDto dto) =>
            new PuestoElectivoViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };
    }
}
