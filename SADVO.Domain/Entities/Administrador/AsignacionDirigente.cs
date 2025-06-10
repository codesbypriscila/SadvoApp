namespace SADVO.Domain.Entities.Administrador
{
    public class AsignacionDirigente
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int PartidoPoliticoId { get; set; }
        public PartidoPolitico PartidoPolitico { get; set; } = null!;
    }

}