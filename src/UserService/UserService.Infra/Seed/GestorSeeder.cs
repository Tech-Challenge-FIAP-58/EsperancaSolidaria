using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using UserService.Domain.Interfaces.Utils;
using UserService.Domain.Models;
using UserService.Infra.Mongo.Collections;

namespace UserService.Infra.Seed
{
    public static class GestorSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("GestorSeeder");
            var collections = sp.GetRequiredService<MongoCollections>();
            var configuration = sp.GetRequiredService<IConfiguration>();
            var passwordHasher = sp.GetRequiredService<IPasswordHasher>();

            logger.LogInformation("Iniciando seed do GestorONG.");

            var hasGestor = await collections.Users
                .Find(u => u.Roles.Contains(Roles.GestorONG))
                .AnyAsync();

            if (hasGestor)
            {
                logger.LogInformation("Já existe um GestorONG. Seed ignorado.");
                return;
            }

            var cpf = configuration["BootstrapGestor:Cpf"];
            var email = configuration["BootstrapGestor:Email"];
            var password = configuration["BootstrapGestor:Password"];
            var name = configuration["BootstrapGestor:Name"] ?? "Gestor ONG";

            if (string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(cpf))
            {
                logger.LogError("Configuração BootstrapGestor incompleta. Seed abortado.");
                return;
            }

            var gestor = new User
            {
                Guid = Guid.NewGuid(),
                Name = name,
                Email = email,
                Cpf = cpf,
                Password = passwordHasher.Hash(password),
                Roles = new List<string> { Roles.GestorONG }
            };

            await collections.Users.InsertOneAsync(gestor);

            logger.LogInformation("GestorONG criado com sucesso (Guid: {Guid}).", gestor.Guid);
        }
    }
}
