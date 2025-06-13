namespace SADVO.Application.Dtos
{
    public class CuidadanoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Cedula { get; set; } = null!; 
        public bool Activo { get; set; }

    }
}