using SADVO.Application.Dtos;
using SADVO.Domain.Entities;
using SADVO.Domain.Entities.Administrador;
using System.ComponentModel.DataAnnotations;

namespace SADVO.Application.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(15)]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(20)]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(15)]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        public IEnumerable<Rol> RolesDisponibles { get; set; } = new List<Rol>();

        [DataType(DataType.Password)]
        [StringLength(100)]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }

        public bool Activo { get; set; }

        public UsuarioDto ToDto() => new UsuarioDto
        {
            Id = Id,
            Nombre = Nombre,
            Apellido = Apellido,
            Email = Email,
            Username = Username,
            RolId = RolId,
            Activo = Activo
        };

        public static UsuarioViewModel FromDto(UsuarioDto dto) => new UsuarioViewModel
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            Username = dto.Username,
            RolId = dto.RolId,
            Activo = dto.Activo
        };
    }
}