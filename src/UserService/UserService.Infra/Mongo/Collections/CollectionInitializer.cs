using MongoDB.Driver;
using UserService.Infra.Mongo.Collections;

public sealed class MongoCollectionInitializer
{
    private readonly IMongoDatabase _database;

    public MongoCollectionInitializer(
        IMongoDatabase database)
    {
        _database = database;
    }

    public async Task InitializeAsync()
    {
        var collections =
            await _database
                .ListCollectionNames()
                .ToListAsync();

        if (!collections.Contains(CollectionsNames.Users))
        {
            await _database.CreateCollectionAsync(
                CollectionsNames.Users);
        }

        if (!collections.Contains(
                CollectionsNames.UserStatistics))
        {
            await _database.CreateCollectionAsync(
                CollectionsNames.UserStatistics);
        }

        if (!collections.Contains(
                CollectionsNames.AuthLogs))
        {
            await _database.CreateCollectionAsync(
                CollectionsNames.AuthLogs);
        }
    }
}