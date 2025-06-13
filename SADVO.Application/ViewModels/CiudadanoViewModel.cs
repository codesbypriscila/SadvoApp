using System.ComponentModel.DataAnnotations;
using SADVO.Application.Dtos;

namespace SADVO.Web.ViewModels
{
    public class CuidadanoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(20, ErrorMessage = "El nombre no puede exceder 20 caracteres")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(30, ErrorMessage = "El apellido no puede exceder 30 caracteres")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un email válido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(13, ErrorMessage = "La cédula no puede exceder 13 caracteres")]
        public string Cedula { get; set; } = null!;

        public bool Activo { get; set; }

        public CuidadanoDto ToDto() => new()
        {
            Id = Id,
            Nombre = Nombre,
            Apellido = Apellido,
            Email = Email,
            Cedula = Cedula,
            Activo = Activo
        };

        public static CuidadanoViewModel FromDto(CuidadanoDto dto) => new()
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
