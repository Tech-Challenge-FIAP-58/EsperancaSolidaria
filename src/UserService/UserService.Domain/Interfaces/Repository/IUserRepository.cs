using UserService.Domain.Models;

namespace UserService.Domain.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User?> GetById(Guid id);
        Task<User?> FindByEmail(string email);
        Task<bool> ExistsByEmail(string email);
        Task<bool> ExistsByCpf(string cpf);
        Task<bool> Create(User user);
        Task<bool> Update(Guid id, User userUpdate);
        Task<bool> Remove(User entity);
    }
}
