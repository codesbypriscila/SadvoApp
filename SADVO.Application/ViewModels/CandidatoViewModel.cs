using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using SADVO.Application.Dtos;

namespace SADVO.Application.ViewModels
{
    public class CandidatoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del candidato es obligatorio.")]
        [StringLength(15, ErrorMessage = "El nombre no debe exceder los 15 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido del candidato es obligatorio.")]
        [StringLength(15, ErrorMessage = "El apellido no debe exceder los 15 caracteres.")]
        public string Apellido { get; set; } = null!;

        public string? FotoUrl { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? FotoFile { get; set; }

        public bool Activo { get; set; }

        [Required(ErrorMessage = "El partido político es obligatorio.")]
        public int PartidoPoliticoId { get; set; }

        public CandidatoDto ToDto() =>
            new CandidatoDto
            {
                Id = Id,
                Nombre = Nombre,
                Apellido = Apellido,
                FotoUrl = FotoUrl,
                Activo = Activo,
                PartidoPoliticoId = PartidoPoliticoId
            };

        public static CandidatoViewModel FromDto(CandidatoDto dto) =>
            new CandidatoViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                FotoUrl = dto.FotoUrl,
                Activo = dto.Activo,
                PartidoPoliticoId = dto.PartidoPoliticoId
            };
    }
}
