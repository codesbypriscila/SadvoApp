using SADVO.Application.Dtos;
using SADVO.Application.ViewModels;

namespace SADVO.Application.Interfaces
{
    public interface ILoginService
    {
        Task<UsuarioDto?> LoginAsync(LoginViewModel loginViewModel);
    }
}