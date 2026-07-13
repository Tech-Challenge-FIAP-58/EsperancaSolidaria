namespace UserService.Domain.Interfaces.Utils
{
    public interface IPasswordHasher
    {
        string Hash(string raw);
        bool Verify(string raw, string hashed);
    }
}
