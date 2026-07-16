using MongoDB.Driver;

namespace CampaignService.Infra.Mongo.Collections
{
    public class MongoCollectionInitializer(
        IMongoDatabase database)
    {
        private readonly IMongoDatabase _database = database;

        public async Task InitializeAsync()
        {
            var collections =
                await _database
                    .ListCollectionNames()
                    .ToListAsync();

            if (!collections.Contains(CollectionsNames.Campaigns))
            {
                await _database.CreateCollectionAsync(
                    CollectionsNames.Campaigns);
            }

            if (!collections.Contains(CollectionsNames.CampaignLogs))
			{
				await _database.CreateCollectionAsync(
					CollectionsNames.CampaignLogs);
			}
		}
    }
}
