using UserService.Domain.Models;

namespace UserService.Domain.Interfaces.Utils
{
    public interface IJwtTokenGenerator
    {
        string Generate(User user);
    }
}
