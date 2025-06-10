using SADVO.Domain.Entities.Elector;

namespace SADVO.Domain.Entities.Administrador
{
    public class Ciudadano
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Cedula { get; set; } = null!; 
        public bool Activo { get; set; }

        public ICollection<Voto> Votos { get; set; } = new List<Voto>();
    }

}