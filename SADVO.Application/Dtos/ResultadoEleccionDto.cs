namespace SADVO.Application.Dtos
{
    public class ResultadoEleccionDto
    {
        public string NombreEleccion { get; set; } = null!;
        public List<ResumenPuestoDto> Puestos { get; set; } = new();
    }
}