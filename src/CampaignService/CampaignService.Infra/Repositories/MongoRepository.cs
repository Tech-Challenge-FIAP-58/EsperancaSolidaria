using CampaignService.Domain.Models;
using CampaignService.Infra.Repositories.Interfaces;
using MongoDB.Driver;

namespace CampaignService.Infra.Repositories
{
	public class MongoRepository<T>(IMongoCollection<T> collection) : IRepository<T> where T : ModelBase
	{
		protected readonly IMongoCollection<T> _collection = collection;

		public void Add(T entity)
		{
			throw new NotImplementedException();
		}

		public void Delete(T entity)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<T>> GetAll()
		{
			throw new NotImplementedException();
		}

		public Task<T?> GetById(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public void Update(T entity)
		{
			throw new NotImplementedException();
		}
	}
}
