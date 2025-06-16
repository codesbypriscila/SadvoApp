using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Domain.Interfaces;
using SADVO.Application.Utils;
using AutoMapper;

namespace SADVO.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IMapper _mapper;

        public LoginService(ILoginRepository loginRepository, IMapper mapper)
        {
            _loginRepository = loginRepository;
            _mapper = mapper;
        }

        public async Task<UsuarioDto?> LoginAsync(LoginViewModel loginViewModel)
        {
            var user = await _loginRepository.GetByEmailAsync(loginViewModel.Email);

            if (user == null)
            {
                return null;
            }

            if (!user.Activo)
            {
                throw new ApplicationException("Usuario inactivo");
            }

            var hashedInput = PasswordHasher.Hash(loginViewModel.Password);
            if (user.PasswordHash != hashedInput)
                return null;

            var dto = _mapper.Map<UsuarioDto>(user);
            dto.Rol = user.RolId switch
            {
                1 => "Administrador",
                2 => "Dirigente",
                _ => "Sin Rol"
            };

            return dto;
        }
    }
}