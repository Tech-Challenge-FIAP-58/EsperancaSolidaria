using UserService.Domain.Models;
using MongoDB.Driver;

namespace UserService.Infra.Mongo.Collections
{
    public sealed class MongoCollections
    {
        public IMongoCollection<User> Users { get; }

        public MongoCollections(
            IMongoDatabase database)
        {
            Users =
                database.GetCollection<User>(
                    CollectionsNames.Users);
        }
    }
}
