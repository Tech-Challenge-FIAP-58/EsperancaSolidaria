using CampaignService.Domain.Models;
using CampaignService.Infra.Repositories.Interfaces;

namespace CampaignService.Infra.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : ModelBase
    {
        public void Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
