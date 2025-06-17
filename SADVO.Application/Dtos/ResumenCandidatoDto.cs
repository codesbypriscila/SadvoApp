namespace SADVO.Application.Dtos
{
    public class ResumenCandidatoDto
    {
        public string NombreCandidato { get; set; } = null!;
        public string Partido { get; set; } = null!;
        public int Votos { get; set; }
        public double Porcentaje { get; set; }
    }
    
}