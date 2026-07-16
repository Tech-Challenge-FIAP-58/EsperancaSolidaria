using DonationService.Infra.Repositories.Interfaces;
using EsperancaSolidaria.Contracts.Entities;
using MongoDB.Driver;

namespace DonationService.Infra
{
	public class MongoRepository<T>(IMongoCollection<T> collection) : IRepository<T> where T : ModelBase
	{
		protected readonly IMongoCollection<T> _collection = collection;

		public async Task Add(T entity)
		{
			await _collection.InsertOneAsync(entity);
		}

		public async Task Delete(T entity)
		{
			await _collection.DeleteOneAsync(e => e.Id == entity.Id);
		}

		public async Task<IEnumerable<T>> GetAll()
		{
			return await _collection.Find(_ => true).ToListAsync();
		}

		public async Task<T?> GetById(Guid id)
		{
			return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
		}

		public async Task Update(T entity)
		{
			await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
		}
	}
}
