using SADVO.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.ViewModels
{
    public class CiudadanoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(20, ErrorMessage = "El nombre no debe exceder los 20 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(20, ErrorMessage = "El apellido no debe exceder los 20 caracteres.")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [StringLength(13, ErrorMessage = "La cédula no debe exceder los 13 caracteres.")]
        public string Cedula { get; set; } = null!;

        public bool Activo { get; set; }

        public CuidadanoDto ToDto() =>
            new CuidadanoDto
            {
                Id = Id,
                Nombre = Nombre,
                Apellido = Apellido,
                Email = Email,
                Cedula = Cedula,
                Activo = Activo
            };

        public static CiudadanoViewModel FromDto(CuidadanoDto dto) =>
            new CiudadanoViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Cedula = dto.Cedula,
                Activo = dto.Activo
            };
    }
}
