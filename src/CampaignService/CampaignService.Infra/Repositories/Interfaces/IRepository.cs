using EsperancaSolidaria.Contracts.Entities;

namespace CampaignService.Infra.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : ModelBase
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity?> GetById(Guid id);
        Task Add(TEntity entity);
		Task Update(TEntity entity);
		Task Delete(TEntity entity);
    }
}
