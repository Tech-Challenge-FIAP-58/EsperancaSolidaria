using CampaignService.Application.Consumers;
using CampaignService.Infra.Configurations;
using CampaignService.Infra.Mongo.Bootstrap;
using CampaignService.Infra.Mongo.Collections;
using CampaignService.Infra.Mongo.Indexes;
using CampaignService.WebApi.Settings;
using MassTransit;
using Microsoft.Extensions.Options;

namespace CampaignService.WebApi.Extensions
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
            builder.Services.AddSingleton<CampaignIndexes>();
            builder.Services.AddSingleton<MongoBootstrap>();

            return builder;
        }

        private static WebApplicationBuilder AddMassTransitSettings(this WebApplicationBuilder builder)
		{
			builder.Services.AddMassTransit(x =>
			{
				// Prefixo por serviço: garante uma fila própria (campaign-donation-received-event),
				// distinta da do UserService, para que o Publish faça fan-out para ambos.
				x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("campaign", false));

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
