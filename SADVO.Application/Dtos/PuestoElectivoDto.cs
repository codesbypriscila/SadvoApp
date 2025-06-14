namespace SADVO.Application.Dtos
{
    public class PuestoElectivoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public bool Activo { get; set; }
    }
}
