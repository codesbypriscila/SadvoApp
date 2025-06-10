using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Enums;

namespace SADVO.Domain.Entities.Dirigente
{
    public class AlianzaPolitica
    {
        public int Id { get; set; }

        public int PartidoSolicitanteId { get; set; }
        public PartidoPolitico PartidoSolicitante { get; set; } = null!;
        public int PartidoReceptorId { get; set; }
        public PartidoPolitico PartidoReceptor { get; set; } = null!;
        public EstadoAlianza Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaRespuesta { get; set; }
    }

}