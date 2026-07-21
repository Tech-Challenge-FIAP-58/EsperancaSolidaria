using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UserService.WebApi.HealthChecks
{
    public sealed class MongoHealthCheck(IMongoDatabase database) : IHealthCheck
    {
        private static readonly BsonDocumentCommand<BsonDocument> PingCommand =
            new(new BsonDocument("ping", 1));

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await database.RunCommandAsync(PingCommand, cancellationToken: cancellationToken);
                return HealthCheckResult.Healthy("Mongo respondeu ao ping.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Mongo não respondeu ao ping.", ex);
            }
        }
    }
}
