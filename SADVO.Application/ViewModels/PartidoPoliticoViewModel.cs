using Microsoft.AspNetCore.Http;
using SADVO.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.ViewModels
{
    public class PartidoPoliticoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(100, ErrorMessage = "La descripción no debe exceder los 100 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Las siglas son obligatorias.")]
        [StringLength(8, ErrorMessage = "Las siglas no deben exceder los 8 caracteres.")]
        public string Siglas { get; set; } = null!;

        public string? LogoUrl { get; set; }
        
        [DataType(DataType.Upload)]
        public IFormFile? LogoFile { get; set; }

        public bool Activo { get; set; }

        public PartidoPoliticoDto ToDto() =>
            new PartidoPoliticoDto
            {
                Id = Id,
                Nombre = Nombre,
                Descripcion = Descripcion ?? string.Empty,
                Siglas = Siglas,
                LogoUrl = LogoUrl ?? string.Empty,
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
