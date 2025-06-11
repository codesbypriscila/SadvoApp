using SADVO.Domain.Entities.Administrador;

namespace SADVO.Domain.Interfaces
{
    public interface ILoginRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);
    }
}
