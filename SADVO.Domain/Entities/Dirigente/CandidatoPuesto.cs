using SADVO.Domain.Entities.Administrador;

namespace SADVO.Domain.Entities.Dirigente
{
    public class CandidatoPuesto
    {
        public int Id { get; set; }

        public int CandidatoId { get; set; }
        public Candidato Candidato { get; set; } = null!;

        public int PuestoElectivoId { get; set; }
        public PuestoElectivo PuestoElectivo { get; set; } = null!;

        public int PartidoPoliticoId { get; set; }
        public PartidoPolitico PartidoPolitico { get; set; } = null!;

        public int? EleccionId { get; set; }
        public Eleccion? Eleccion { get; set; }
    }

}