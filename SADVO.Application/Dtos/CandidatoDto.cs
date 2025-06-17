namespace SADVO.Application.Dtos
{
    public class CandidatoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? FotoUrl { get; set; }
        public bool Activo { get; set; }
        public int PartidoPoliticoId { get; set; }
        public string PartidoNombre { get; set; } = null!;
        
    }
}