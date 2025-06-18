namespace SADVO.Application.Dtos
{
    public class CandidatoPuestoDto
    {
        public int Id { get; set; }
        public int CandidatoId { get; set; }
        public int PuestoElectivoId { get; set; }
        public int PartidoPoliticoId { get; set; }
        public int? EleccionId { get; set; }

        public string NombreCandidato { get; set; } = null!;
        public string ApellidoCandidato { get; set; } = null!;
        public string NombrePuesto { get; set; } = null!;
    }

}