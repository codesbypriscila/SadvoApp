namespace SADVO.Application.Dtos
{
    public class ResumenPuestoDto
    {
        public string NombrePuesto { get; set; } = null!;
        public List<ResumenCandidatoDto> Candidatos { get; set; } = new();
    }
    
}