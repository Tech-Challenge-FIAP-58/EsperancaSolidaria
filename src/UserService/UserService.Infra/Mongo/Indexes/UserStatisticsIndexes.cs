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

        // Índice para as consultas de contagem/soma por usuário.
        // A unicidade da doação já é garantida pelo _id (= DonationId),
        // então não há índice único adicional aqui.
        await stats.Indexes.CreateOneAsync(
            new CreateIndexModel<UserDonationStat>(
                Builders<UserDonationStat>
                    .IndexKeys
                    .Ascending(x => x.UserId)));
    }
}
