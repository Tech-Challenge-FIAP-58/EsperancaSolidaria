using UserService.Domain.Models;

namespace UserService.Domain.Interfaces.Utils
{
    /// <summary>
    /// Emite o token JWT de um usuário. Responsabilidade de autenticação — separada da persistência.
    /// </summary>
    public interface IJwtTokenGenerator
    {
        string Generate(User user);
    }
}
