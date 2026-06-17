using UserService.Infra.Configurations;
using UserService.Infra.DependencyInjection;
using UserService.Infra.Mongo.Bootstrap;
using UserService.Infra.Mongo.Collections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<MongoConfig>()
    .BindConfiguration("Mongo")
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddMongo();

builder.Services.AddSingleton<MongoCollections>();
builder.Services.AddSingleton<MongoCollectionInitializer>();
builder.Services.AddSingleton<UserIndexes>();
builder.Services.AddSingleton<MongoBootstrap>();

var app = builder.Build();

using var scope =
    app.Services.CreateScope();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
