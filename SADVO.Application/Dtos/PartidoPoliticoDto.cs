namespace SADVO.Application.Dtos
{
    public class PartidoPoliticoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string Siglas { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public bool Activo { get; set; }
    }
    
}