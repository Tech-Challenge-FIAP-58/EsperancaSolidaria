using MongoDB.Driver;
using UserService.Domain.Models;
using UserService.Infra.Mongo.Collections;

public sealed class UserIndexes(
    IMongoDatabase database)
{
    private readonly IMongoDatabase _database = database;

    public async Task CreateAsync()
    {
        var users =
            _database.GetCollection<User>(
                CollectionsNames.Users);

        await users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                Builders<User>
                    .IndexKeys
                    .Ascending(x => x.Email),

                new CreateIndexOptions
                {
                    Unique = true
                }));
        await users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                Builders<User>
                    .IndexKeys
                    .Ascending(x => x.Cpf),

                new CreateIndexOptions
                {
                    Unique = true
                }));
    }
}