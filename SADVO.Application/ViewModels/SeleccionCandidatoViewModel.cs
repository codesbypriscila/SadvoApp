namespace SADVO.Application.ViewModels
{
    public class SeleccionCandidatoViewModel
    {
        public int PuestoElectivoId { get; set; }
        public string NombrePuesto { get; set; } = null!;
        public List<CandidatoOpcionViewModel> Candidatos { get; set; } = new();
    }

}
