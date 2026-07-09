using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Models;
using UserService.Infra.Mongo;
using MongoDB.Driver;

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

        public async Task<bool> Register(T entity) =>
            await ExecuteWrite(async () =>
            {
                await _collection.InsertOneAsync(entity);
                return true;
            });

        public async Task<bool> Edit(T entity) =>
            await ExecuteWrite(async () =>
            {
                var result = await _collection.ReplaceOneAsync(e => e.Guid == entity.Guid, entity);
                return result.IsAcknowledged;
            });

        public async Task<bool> Delete(T entity)
        {
            var result = await _collection.DeleteOneAsync(e => e.Guid == entity.Guid);
            return result.IsAcknowledged;
        }

        /// <summary>
        /// Executa uma escrita traduzindo violação de índice único do Mongo
        /// em exceção de domínio. O driver não expõe um ponto único de commit,
        /// então a tradução vive aqui, em volta de cada operação.
        /// </summary>
        private static async Task<bool> ExecuteWrite(Func<Task<bool>> write)
        {
            try
            {
                return await write();
            }
            catch (MongoWriteException ex) when (ex.IsDuplicateKey())
            {
                throw ex.ToDuplicateEntityException();
            }
        }
    }
}
