using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Elector;

namespace SADVO.Domain.Entities.Dirigente
{
    public class Candidato
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? FotoUrl { get; set; }
        public bool Activo { get; set; }
        public int PartidoPoliticoId { get; set; }
        public PartidoPolitico PartidoPolitico { get; set; } = null!;

        public ICollection<CandidatoPuesto> Puestos { get; set; } = new List<CandidatoPuesto>();
        public ICollection<Voto> Votos { get; set; } = new List<Voto>();
    }

}