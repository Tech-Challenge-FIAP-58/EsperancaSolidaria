using DonationService.WebApi.Settings;
using MassTransit;
using Microsoft.Extensions.Options;

namespace DonationService.WebApi.Extensions
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

			return builder;
		}

		private static WebApplicationBuilder AddMassTransitSettings(this WebApplicationBuilder builder)
		{
			builder.Services.AddMassTransit(x =>
			{
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
