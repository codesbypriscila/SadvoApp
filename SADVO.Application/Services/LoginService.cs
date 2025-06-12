using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Domain.Interfaces;
using SADVO.Application.Utils;

namespace SADVO.Application.Services
{

    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;

        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public async Task<UsuarioDto?> LoginAsync(LoginViewModel loginViewModel)
        {
            var user = await _loginRepository.GetByEmailAsync(loginViewModel.Email);

            if (user == null || !user.Activo)
                return null;

            var hashedInput = PasswordHasher.Hash(loginViewModel.Password);
            if (user.PasswordHash != hashedInput)
                return null;

            return new UsuarioDto
            {
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                Rol = user.Rol.Nombre
            };
        }
    }

}