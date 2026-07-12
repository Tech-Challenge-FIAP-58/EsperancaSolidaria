using MongoDB.Driver;
using UserService.Domain.Models;
using UserService.Infra.Mongo.Collections;

public sealed class UserStatisticsIndexes(
    IMongoDatabase database)
{
    private readonly IMongoDatabase _database = database;

    public async Task CreateAsync()
    {
        var stats =
            _database.GetCollection<UserDonationStat>(
                CollectionsNames.UserStatistics);

        // Único: 1 documento de rollup por usuário.
        await stats.Indexes.CreateOneAsync(
            new CreateIndexModel<UserDonationStat>(
                Builders<UserDonationStat>
                    .IndexKeys
                    .Ascending(x => x.UserId),
                new CreateIndexOptions { Unique = true }));

        var processed =
            _database.GetCollection<ProcessedMessage>(
                CollectionsNames.ProcessedMessages);

        await processed.Indexes.CreateOneAsync(
            new CreateIndexModel<ProcessedMessage>(
                Builders<ProcessedMessage>
                    .IndexKeys
                    .Ascending(x => x.ProcessedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(30) }));
    }
}
