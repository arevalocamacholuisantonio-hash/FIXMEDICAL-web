using BCrypt.Net;

namespace FixMedicalWeb.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string  HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor: 12);
        }
        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }

    }
}
