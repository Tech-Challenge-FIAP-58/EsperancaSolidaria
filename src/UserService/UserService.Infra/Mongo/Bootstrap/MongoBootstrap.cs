namespace UserService.Infra.Mongo.Bootstrap;

public sealed class MongoBootstrap(
    MongoCollectionInitializer collections,
    UserIndexes indexes,
    UserStatisticsIndexes statisticsIndexes)
{
    private readonly MongoCollectionInitializer _collections = collections;

    private readonly UserIndexes _indexes = indexes;

    private readonly UserStatisticsIndexes _statisticsIndexes = statisticsIndexes;

    public async Task InitializeAsync()
    {
        await _collections.InitializeAsync();

        await _indexes.CreateAsync();

        await _statisticsIndexes.CreateAsync();
    }
}