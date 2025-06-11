using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using SADVO.Infrastructure.AppDbContext;

namespace SADVO.Infrastructure.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly ApplicationDbContext _context;

        public LoginRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
