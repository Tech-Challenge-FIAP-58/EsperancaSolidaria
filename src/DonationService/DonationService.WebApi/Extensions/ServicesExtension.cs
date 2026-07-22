using DonationService.Application.Producer;
using DonationService.Application.Services;
using DonationService.Domain.Interfaces;
using DonationService.Domain.Interfaces.MassTransit.Producer;
using DonationService.Infra;
using DonationService.Infra.Repositories.Interfaces;
using DonationService.WebApi.Middleware;

namespace DonationService.WebApi.Extensions
{
	public static class ServicesExtension
	{
		public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
		{
			// =================================== Add controllers =================================== //
			builder.Services.AddControllers();

			// =================================== Add exception handling =================================== //
			builder.Services.AddProblemDetails();
			builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

			// =================================== Add health checks =================================== //
			builder.Services.AddHealthChecks();

			// =================================== Add repositories =================================== //
			builder.AddRepositories();

			// =================================== Add useCases =================================== //
			builder.AddUseCases();

			// =================================== Add event producers =================================== //
			builder.Services.AddScoped<IDonationReceivedEventProducer, DonationReceivedEventProducer>();

			// =================================== Add swagger =================================== //
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			return builder;
		}

		private static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
		{
			builder.Services.AddScoped<IDonationLogRepository, DonationLogRepository>();
			builder.Services.AddScoped<IDonationRepository, DonationRepository>();

			return builder;
		}

		private static WebApplicationBuilder AddUseCases(this WebApplicationBuilder builder)
		{
			builder.Services.AddScoped<IDonationReceivedEventProducer, DonationReceivedEventProducer>();
			builder.Services.AddScoped<ICampaignDonationServiceDonationService, CampaignDonationService>();

			return builder;
		}
	}
}
