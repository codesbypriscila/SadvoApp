namespace SADVO.Application.Interfaces
{
    public interface IEleccionService
    {
        Task<bool> HayEleccionActivaAsync();
    }
}
