using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserService.Application.Auth;
using UserService.Application.Mapping;
using UserService.Application.Services;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Utils;
using UserService.Infra.Configurations;
using UserService.Infra.Repository;
using UserService.Infra.Utils;
using UserService.WebApi.HealthChecks;
using UserService.WebApi.Middleware;

namespace UserService.WebApi.Extensions
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
			builder.AddAppHealthChecks();

			// =================================== Add AutoMapper =================================== //
			builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

			// =================================== Add repositories =================================== //
			builder.AddRepositories();

			// =================================== Add useCases =================================== //
			builder.AddUseCases();

			// =================================== Add auth (JWT) =================================== //
			builder.AddJwtAuth();

			// =================================== Add swagger =================================== //
			builder.AddSwagger();

			return builder;
		}

		/// <summary>
		/// Registra as dependências externas que a aplicação usa hoje.
		/// O Mongo é a única: o bus do MassTransit sobe mas nenhum endpoint depende dele
		/// enquanto o Worker de doações não existir.
		/// </summary>
		private static WebApplicationBuilder AddAppHealthChecks(this WebApplicationBuilder builder)
		{
			builder.Services
				.AddHealthChecks()
				.AddCheck<MongoHealthCheck>("mongo", tags: [HealthCheckTags.Dependency]);

			return builder;
		}

		private static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
		{
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

			return builder;
		}

		private static WebApplicationBuilder AddUseCases(this WebApplicationBuilder builder)
		{
			builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

			return builder;
		}

		private static WebApplicationBuilder AddJwtAuth(this WebApplicationBuilder builder)
		{
			// Settings de JWT tipados e validados na inicialização.
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
	}
}
