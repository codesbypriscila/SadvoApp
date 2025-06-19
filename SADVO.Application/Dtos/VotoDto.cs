namespace SADVO.Application.Dtos
{
    public class VotoDto
    {
        public int CiudadanoId { get; set; }
        public int CandidatoId { get; set; }
        public int EleccionId { get; set; }
        public int PuestoElectivoId { get; set; }
        public DateTime FechaVoto { get; set; }
    }
}
