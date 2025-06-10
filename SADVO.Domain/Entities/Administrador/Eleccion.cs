using SADVO.Domain.Entities.Dirigente;
using SADVO.Domain.Entities.Elector;

namespace SADVO.Domain.Entities.Administrador
{
    public class Eleccion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public bool Activa { get; set; }

        public ICollection<Voto> Votos { get; set; } = new List<Voto>();
        public ICollection<CandidatoPuesto> Candidatos { get; set; } = new List<CandidatoPuesto>();
    }

}