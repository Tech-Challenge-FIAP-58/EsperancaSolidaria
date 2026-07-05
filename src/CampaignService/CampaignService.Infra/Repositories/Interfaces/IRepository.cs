using CampaignService.Domain.Models;

namespace CampaignService.Infra.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : ModelBase
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity?> GetById(Guid id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
