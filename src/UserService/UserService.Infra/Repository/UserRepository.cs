using MongoDB.Driver;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Models;
using UserService.Infra.Mongo.Collections;

namespace UserService.Infra.Repository
{
    public class UserRepository(MongoCollections collections)
        : MongoRepository<User>(collections.Users), IUserRepository
    {
        public async Task<IEnumerable<User>> GetAll() => await Get();

        public async Task<User?> GetById(Guid id) => await Get(id);

        public async Task<User?> FindByEmail(string email) =>
            await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task<bool> ExistsByEmail(string email) =>
            await _collection.Find(u => u.Email == email).AnyAsync();

        public async Task<bool> ExistsByCpf(string cpf) =>
            await _collection.Find(u => u.Cpf == cpf).AnyAsync();

        public async Task<bool> Create(User user) => await Register(user);

        public async Task<bool> Update(Guid id, User userUpdate) => await Edit(userUpdate);

        public async Task<bool> Remove(User entity) => await Delete(entity);
    }
}
