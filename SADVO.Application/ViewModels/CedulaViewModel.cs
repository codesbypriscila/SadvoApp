using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.ViewModels
{
    public class CedulaViewModel
    {
        [Required(ErrorMessage = "La cédula es obligatoria")]
        [RegularExpression(@"^\d{3}-\d{7}-\d{1}$", ErrorMessage = "Formato inválido")]
        public string Cedula { get; set; } = null!;
    }
}
