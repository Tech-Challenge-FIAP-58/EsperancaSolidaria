using CampaignService.Infra.Mongo.Bootstrap;
using CampaignService.WebApi.Extensions;
using CampaignService.WebApi.Metrics;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// =================================== Add settings =================================== //
builder.AddApiSettings();

// =================================== Add services =================================== //
builder.AddServices();

// =================================== Add app config =================================== //
var app = builder.Build();

using var scope =
    app.Services.CreateScope();

// =================================== Initialize Mongo DB =================================== //
var bootstrap =
    scope.ServiceProvider
        .GetRequiredService<MongoBootstrap>();

await bootstrap.InitializeAsync();

// Configure the HTTP request pipeline.
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

app.UseAuthorization();

app.UseHttpMetrics();

app.MapMetrics();

app.MapControllers();

app.Run();
