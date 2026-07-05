using CampaignService.Domain.Models;
using MongoDB.Driver;

namespace CampaignService.Infra.Mongo.Collections
{
    public class MongoCollections(
        IMongoDatabase database)
    {
        public IMongoCollection<Campaign> Campaigns { get; } =
                database.GetCollection<Campaign>(
                    CollectionsNames.Campaigns);
    }
}
