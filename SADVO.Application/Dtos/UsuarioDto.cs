namespace SADVO.Application.Dtos
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public int RolId { get; set; }
        public string Rol { get; set; } = null!;
        public bool Activo { get; set; }
        public int? PartidoPoliticoId { get; set; } 

    }
}