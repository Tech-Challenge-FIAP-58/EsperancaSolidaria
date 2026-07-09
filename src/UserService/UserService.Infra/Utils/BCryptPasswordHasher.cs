using UserService.Domain.Interfaces.Utils;

namespace UserService.Infra.Utils
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string raw) => BCrypt.Net.BCrypt.HashPassword(raw);

        public bool Verify(string raw, string hashed) => BCrypt.Net.BCrypt.Verify(raw, hashed);
    }
}
