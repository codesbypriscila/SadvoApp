using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.ViewModels
{
    public class ValidarIdentidadViewModel
    {
        [Required]
        public string CedulaDigitada { get; set; } = null!;

        [Required(ErrorMessage = "Debe subir una imagen de su cédula")]
        public IFormFile ImagenCedula { get; set; } = null!;
    }
}
