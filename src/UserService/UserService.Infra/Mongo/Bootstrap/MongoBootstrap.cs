namespace UserService.Infra.Mongo.Bootstrap;

public sealed class MongoBootstrap(
    MongoCollectionInitializer collections,
    UserIndexes indexes)
{
    private readonly MongoCollectionInitializer _collections = collections;

    private readonly UserIndexes _indexes = indexes;

    public async Task InitializeAsync()
    {
        await _collections.InitializeAsync();

        await _indexes.CreateAsync();
    }
}