namespace SADVO.Application.Dtos
{
    public class EleccionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public DateTime FechaRealizacion { get; set; }
        public bool Activa { get; set; }
        public int CantidadPartidosPoliticos { get; set; }
        public int CantidadPuestosDisputados { get; set; }
        public bool Finalizada { get; set; }
    } 
}
