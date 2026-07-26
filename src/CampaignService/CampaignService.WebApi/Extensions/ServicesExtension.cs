using System.Security.Claims;
using System.Text;
using CampaignService.Application.Producer;
using CampaignService.Application.Services;
using CampaignService.Domain.Interfaces;
using CampaignService.Domain.Interfaces.MassTransit.Producer;
using CampaignService.Infra.Repositories;
using CampaignService.Infra.Repositories.Interfaces;
using CampaignService.WebApi.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

			// =================================== Add event producers =================================== //
			builder.Services.AddScoped<IDonationRejectedEventProducer, DonationRejectedEventProducer>();

			// =================================== Add useCases =================================== //
			builder.AddUseCases();

			// =================================== Add auth (JWT) =================================== //
			builder.AddJwtAuth();

			// =================================== Add swagger =================================== //
			builder.AddSwagger();

			return builder;
		}

		private static WebApplicationBuilder AddJwtAuth(this WebApplicationBuilder builder)
		{
			builder.Services
				.AddOptions<JwtSettings>()
				.BindConfiguration("Jwt")
				.ValidateDataAnnotations()
				.ValidateOnStart();

			var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
				?? throw new InvalidOperationException("Seção 'Jwt' ausente no appsettings.");

			builder.Services
				.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(opt =>
				{
					opt.RequireHttpsMetadata = false;
					opt.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ClockSkew = TimeSpan.Zero,
						ValidIssuer = jwt.Issuer,
						ValidAudience = jwt.Audience,
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(jwt.Key)),
						RoleClaimType = ClaimTypes.Role
					};
				});

			builder.Services.AddAuthorization();

			return builder;
		}

		private static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
		{
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = JwtBearerDefaults.AuthenticationScheme,
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Insira apenas o token JWT (o prefixo 'Bearer' é adicionado automaticamente)."
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = JwtBearerDefaults.AuthenticationScheme
							}
						},
						Array.Empty<string>()
					}
				});
			});

			return builder;
		}

		private static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
		{
			builder.Services.AddScoped<ICampaignLogRepository, CampaignLogRepository>();
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
