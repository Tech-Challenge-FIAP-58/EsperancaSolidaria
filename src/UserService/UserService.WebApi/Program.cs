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

// Primeiro do pipeline: captura exceções de todos os middlewares abaixo.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Liveness: o processo está de pé? Nenhum check roda — se responde, está vivo.
// Reiniciar o pod não resolve dependência externa fora do ar, por isso "live" as ignora.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = HealthCheckResponse.WriteAsync
});

// Readiness e /health rodam as dependências que a aplicação realmente usa.
// A tag é allowlist: só entra aqui quem foi marcado em AddAppHealthChecks.
// Hoje isso é o Mongo. Quando o Worker consumir do RabbitMQ de fato, marcar
// o check do bus com a mesma tag.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(HealthCheckTags.Dependency),
    ResponseWriter = HealthCheckResponse.WriteAsync
});

// Rota exigida pelo enunciado.
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(HealthCheckTags.Dependency),
    ResponseWriter = HealthCheckResponse.WriteAsync
});

app.Run();
