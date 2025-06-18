namespace SADVO.Application.Dtos
{
    public class AlianzaPoliticaDto
    {
        public int Id { get; set; }

        public int PartidoSolicitanteId { get; set; }
        public string PartidoSolicitanteNombre { get; set; } = null!;
        public string PartidoSolicitanteSiglas { get; set; } = null!;
        public int PartidoReceptorId { get; set; }
        public string PartidoReceptorNombre { get; set; } = null!;
        public string PartidoReceptorSiglas { get; set; } = null!;

        public string Estado { get; set; } = null!;
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaRespuesta { get; set; }
    }
}
