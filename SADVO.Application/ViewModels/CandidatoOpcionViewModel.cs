namespace SADVO.Application.ViewModels
{
    public class CandidatoOpcionViewModel
    {
        public int CandidatoId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Partido { get; set; } = null!;
        public string? Foto { get; set; }
        public bool Seleccionado { get; set; }
    }
}
