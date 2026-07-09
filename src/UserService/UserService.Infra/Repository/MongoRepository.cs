using MongoDB.Driver;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Models;

namespace UserService.Infra.Repository
{
    public class MongoRepository<T> : IRepository<T> where T : EntityBase
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public async Task<IEnumerable<T>> Get() =>
            await _collection.Find(FilterDefinition<T>.Empty).ToListAsync();

        public async Task<T?> Get(Guid id) =>
            await _collection.Find(entity => entity.Guid == id).FirstOrDefaultAsync();

        public async Task<bool> Register(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return true;
        }

        public async Task<bool> Edit(T entity)
        {
            var result = await _collection.ReplaceOneAsync(e => e.Guid == entity.Guid, entity);
            return result.IsAcknowledged;
        }

        public async Task<bool> Delete(T entity)
        {
            var result = await _collection.DeleteOneAsync(e => e.Guid == entity.Guid);
            return result.IsAcknowledged;
        }
    }
}
