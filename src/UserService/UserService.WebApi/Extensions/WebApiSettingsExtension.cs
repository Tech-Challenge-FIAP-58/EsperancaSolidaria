using MassTransit;
using Microsoft.Extensions.Options;
using UserService.Application.Consumers;
using UserService.Infra.Configurations;
using UserService.Infra.Mongo.Bootstrap;
using UserService.Infra.Mongo.Collections;
using UserService.WebApi.Settings;

namespace UserService.WebApi.Extensions
{
	public static class WebApiSettingsExtension
	{
		public static WebApplicationBuilder AddApiSettings(this WebApplicationBuilder builder)
		{
			builder.Services.Configure<RabbitMqSettings>(
			   builder.Configuration.GetSection("RabbitMQ"));

			RetrySettings.MaxRetryAttempts = builder.Configuration.GetValue<int>("MassTransit:RetrySettings:MaxRetryAttempts");
			RetrySettings.DelayBetweenRetriesInSeconds = builder.Configuration.GetValue<int>("MassTransit:RetrySettings:DelayBetweenRetriesInSeconds");

			builder.AddMassTransitSettings();
			builder.AddDatabaseSettings();

			return builder;
		}

		private static WebApplicationBuilder AddDatabaseSettings(this WebApplicationBuilder builder)
		{
			builder.Services
				.AddOptions<MongoConfig>()
				.BindConfiguration("Mongo")
				.ValidateDataAnnotations()
				.ValidateOnStart();
			builder.AddMongo();

			builder.Services.AddSingleton<MongoCollections>();
			builder.Services.AddSingleton<MongoCollectionInitializer>();
			builder.Services.AddSingleton<UserIndexes>();
			builder.Services.AddSingleton<UserStatisticsIndexes>();
			builder.Services.AddSingleton<MongoBootstrap>();

			return builder;
		}

		private static WebApplicationBuilder AddMassTransitSettings(this WebApplicationBuilder builder)
		{
			builder.Services.AddMassTransit(x =>
			{
				x.AddConsumer<DonationReceivedEventConsumer>();

				x.UsingRabbitMq((context, cfg) =>
				{
					var rabbitSettings = context
						.GetRequiredService<IOptions<RabbitMqSettings>>()
						.Value;

					cfg.Host(rabbitSettings.Host, rabbitSettings.Port, rabbitSettings.VirtualHost, h =>
					{
						h.Username(rabbitSettings.Username);
						h.Password(rabbitSettings.Password);
					});

					cfg.UseMessageRetry(r =>
					{
						r.Interval(
							RetrySettings.MaxRetryAttempts,
							TimeSpan.FromSeconds(RetrySettings.DelayBetweenRetriesInSeconds)
						);
					});

					cfg.ConfigureEndpoints(context);
				});
			});

			return builder;
		}
	}
}
