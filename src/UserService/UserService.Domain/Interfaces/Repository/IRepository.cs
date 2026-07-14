using UserService.Domain.Models;

namespace UserService.Domain.Interfaces.Repository
{
    public interface IRepository<T> where T : EntityBase
    {
        Task<IEnumerable<T>> Get();
        Task<T?> Get(Guid id);
        Task<bool> Register(T entity);
        Task<bool> Edit(T entity);
        Task<bool> Delete(T entity);
    }
}
