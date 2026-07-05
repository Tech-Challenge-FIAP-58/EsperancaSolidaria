using CampaignService.Infra.Mongo.Collections;
using CampaignService.Infra.Mongo.Indexes;

namespace CampaignService.Infra.Mongo.Bootstrap
{
    public sealed class MongoBootstrap(
        MongoCollectionInitializer collections,
        CampaignIndexes indexes)
    {
        private readonly MongoCollectionInitializer _collections = collections;

        private readonly CampaignIndexes _indexes = indexes;

        public async Task InitializeAsync()
        {
            await _collections.InitializeAsync();

            await _indexes.CreateAsync();
        }
    }
}
