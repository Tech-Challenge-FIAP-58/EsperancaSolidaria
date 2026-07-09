using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using UserService.Infra.Configurations;

namespace UserService.WebApi.Extensions
{
    public static class MongoModuleExtension
    {
        public static WebApplicationBuilder AddMongo(
			this WebApplicationBuilder builder)
        {
			// Driver v3 não tem GuidRepresentation padrão: registra o formato Standard globalmente.
			BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

			// Ignora campos presentes no documento mas ausentes na classe (evolução de schema).
			ConventionRegistry.Register(
				"IgnoreExtraElements",
				new ConventionPack { new IgnoreExtraElementsConvention(true) },
				_ => true);

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
