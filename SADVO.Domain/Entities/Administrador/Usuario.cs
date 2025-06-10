namespace SADVO.Domain.Entities.Administrador
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool Activo { get; set; }

        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;

        public AsignacionDirigente? AsignacionDirigente { get; set; }
    }

}