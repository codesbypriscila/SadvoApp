using SADVO.Domain.Entities.Dirigente;

namespace SADVO.Domain.Entities.Administrador
{
    public class PartidoPolitico
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; } 
        public string Siglas { get; set; } = null!; 
        public string? LogoUrl { get; set; }
        public bool Activo { get; set; }

        public ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
        public ICollection<AsignacionDirigente> Dirigentes { get; set; } = new List<AsignacionDirigente>();
    }

}