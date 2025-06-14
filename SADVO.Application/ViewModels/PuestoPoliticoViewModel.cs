using SADVO.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.ViewModels
{
    public class PartidoPoliticoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(200, ErrorMessage = "La descripción no debe exceder los 200 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Las siglas son obligatorias.")]
        [StringLength(10, ErrorMessage = "Las siglas no deben exceder los 10 caracteres.")]
        public string Siglas { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public bool Activo { get; set; }

        public PartidoPoliticoDto ToDto() =>
            new PartidoPoliticoDto
            {
                Id = Id,
                Nombre = Nombre,
                Descripcion = Descripcion ?? string.Empty,
                Siglas = Siglas,
                LogoUrl = LogoUrl,
                Activo = Activo
            };

        public static PartidoPoliticoViewModel FromDto(PartidoPoliticoDto dto) =>
            new PartidoPoliticoViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Siglas = dto.Siglas,
                LogoUrl = dto.LogoUrl,
                Activo = dto.Activo
            };
    }
}
