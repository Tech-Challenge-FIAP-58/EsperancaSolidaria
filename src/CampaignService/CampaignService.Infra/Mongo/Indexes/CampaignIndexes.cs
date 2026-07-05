using CampaignService.Domain.Models;
using CampaignService.Infra.Mongo.Collections;
using MongoDB.Driver;

namespace CampaignService.Infra.Mongo.Indexes
{
    public class CampaignIndexes(
        IMongoDatabase database)
    {
        private readonly IMongoDatabase _database = database;

        public async Task CreateAsync()
        {
            var collection = _database.
                GetCollection<Campaign>(CollectionsNames.Campaigns);

            await collection.Indexes.CreateOneAsync(
               new CreateIndexModel<Campaign>(
                   Builders<Campaign>
                       .IndexKeys
                       .Ascending(x => x.Id),

                   new CreateIndexOptions
                   {
                       Unique = true
                   }));
        }
    }
}
