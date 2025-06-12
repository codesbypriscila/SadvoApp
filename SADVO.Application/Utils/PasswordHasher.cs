using System.Security.Cryptography;
using System.Text;

namespace SADVO.Application.Utils
{
    public static class PasswordHasher
    {
        public static string Hash(string password) 
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes); 
        }
    }
}
