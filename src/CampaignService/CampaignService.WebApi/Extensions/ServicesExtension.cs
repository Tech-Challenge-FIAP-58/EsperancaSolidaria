using CampaignService.Application.Services;
using CampaignService.Domain.Interfaces;
using CampaignService.Infra.Repositories;
using CampaignService.Infra.Repositories.Interfaces;

namespace CampaignService.WebApi.Extensions
{
	public static class ServicesExtension
	{
		public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
		{
			// =================================== Add controllers =================================== //
			builder.Services.AddControllers();

			// =================================== Add repositories =================================== //
			builder.AddRepositories();

			// =================================== Add useCases =================================== //
			builder.AddUseCases();

			// =================================== Add swagger =================================== //
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			return builder;
		}

		private static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
		{
			builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();

			return builder;
		}

		private static WebApplicationBuilder AddUseCases(this WebApplicationBuilder builder)
		{
			builder.Services.AddScoped<ICampaignManagementService, CampaignManagementService>();

			return builder;
		}
	}
}
