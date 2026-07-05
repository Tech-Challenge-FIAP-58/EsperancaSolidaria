using UserService.Infra.Mongo.Bootstrap;
using UserService.WebApi.Extensions;

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
