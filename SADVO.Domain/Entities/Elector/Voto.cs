using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Dirigente;

namespace SADVO.Domain.Entities.Elector
{
    public class Voto
    {
        public int Id { get; set; }

        public int CiudadanoId { get; set; }
        public Ciudadano Ciudadano { get; set; } = null!;

        public int CandidatoId { get; set; }
        public Candidato Candidato { get; set; } = null!;

        public int EleccionId { get; set; }
        public Eleccion Eleccion { get; set; } = null!;

        public int PuestoElectivoId { get; set; }
        public PuestoElectivo PuestoElectivo { get; set; } = null!;

        public DateTime FechaVoto { get; set; }
    }

}