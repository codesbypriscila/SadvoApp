using SADVO.Domain.Entities.Dirigente;

namespace SADVO.Domain.Entities.Administrador
{
    public class PuestoElectivo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public bool Activo { get; set; }

        public ICollection<CandidatoPuesto> Candidatos { get; set; } = new List<CandidatoPuesto>();
    }

}