using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserService.Infra.Configurations;

namespace UserService.Infra.DependencyInjection
{
    public static class MongoModule
    {
        public static IServiceCollection AddMongo(
            this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings =
                    sp.GetRequiredService<IOptions<MongoConfig>>().Value;

                return new MongoClient(settings.ConnectionString);
            });

            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var settings =
                    sp.GetRequiredService<IOptions<MongoConfig>>().Value;

                var client =
                    sp.GetRequiredService<IMongoClient>();

                return client.GetDatabase(settings.Database);
            });

            return services;
        }
    }
}
