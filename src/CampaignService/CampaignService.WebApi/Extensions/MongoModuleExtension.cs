using CampaignService.Infra.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CampaignService.WebApi.Extensions
{
    public static class MongoModuleExtension
    {
        public static WebApplicationBuilder AddMongo(
            this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var settings =
                    sp.GetRequiredService<IOptions<MongoConfig>>().Value;

                return new MongoClient(settings.ConnectionString);
            });

            builder.Services.AddSingleton<IMongoDatabase>(sp =>
            {
                var settings =
                    sp.GetRequiredService<IOptions<MongoConfig>>().Value;

                var client =
                    sp.GetRequiredService<IMongoClient>();

                return client.GetDatabase(settings.Database);
            });

            return builder;
        }
    }
}
