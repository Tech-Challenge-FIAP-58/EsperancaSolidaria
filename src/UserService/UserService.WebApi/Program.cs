using Prometheus;
using CampaignService.WebApi.Metrics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using UserService.Infra.Mongo.Bootstrap;
using UserService.Infra.Seed;
using UserService.WebApi.Extensions;
using UserService.WebApi.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// =================================== Add settings =================================== //
builder.AddApiSettings();

// =================================== Add services =================================== //
builder.AddServices();

// =================================== Add app config =================================== //
var app = builder.Build();

using var scope =
    app.Services.CreateScope();

var bootstrap =
    scope.ServiceProvider
        .GetRequiredService<MongoBootstrap>();

await bootstrap.InitializeAsync();

// Cria o GestorONG padrão (bootstrap) caso ainda não exista.
await GestorSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    AppMetrics.TotalRequests.Inc();
    await next();
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpMetrics();

app.MapMetrics();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = HealthCheckResponse.WriteAsync
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(HealthCheckTags.Dependency),
    ResponseWriter = HealthCheckResponse.WriteAsync
});

// Rota exigida pelo pdf de requisitos do projeto, mas não é usada pelo k8s.
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(HealthCheckTags.Dependency),
    ResponseWriter = HealthCheckResponse.WriteAsync
});

app.Run();
