using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using UserService.Domain.Models;
using UserService.Infra.Configurations;
using UserService.Infra.Utils;

namespace UserService.Test
{
    public class JwtTokenGeneratorTests
    {
        private static JwtTokenGenerator CreateGenerator()
        {
            var settings = new JwtSettings
            {
                Key = "chave-de-teste-com-mais-de-32-bytes-para-hmac-sha256!!",
                Issuer = "test_issuer",
                Audience = "test_audience",
                Expiration = 60
            };
            return new JwtTokenGenerator(Options.Create(settings));
        }

        private static User CreateUser() => new()
        {
            Guid = Guid.NewGuid(),
            Name = "Fulano",
            Email = "fulano@test.com",
            Password = "hash",
            Cpf = "52998224725",
            Roles = new List<string> { Roles.GestorONG }
        };

        [Fact]
        public void Generate_ReturnsNonEmptyJwt()
        {
            var token = CreateGenerator().Generate(CreateUser());

            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Equal(3, token.Split('.').Length); // header.payload.signature
        }

        [Fact]
        public void Generate_IncludesExpectedClaims()
        {
            var user = CreateUser();
            var token = CreateGenerator().Generate(user);

            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

            Assert.Equal("test_issuer", jwt.Issuer);
            Assert.Equal(user.Guid.ToString(), jwt.GetClaim(JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal(user.Email, jwt.GetClaim(JwtRegisteredClaimNames.Email).Value);
            Assert.Equal(user.Name, jwt.GetClaim(JwtRegisteredClaimNames.UniqueName).Value);
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == Roles.GestorONG);
        }

        [Fact]
        public void Generate_MultipleRoles_IncludesAllRoleClaims()
        {
            var user = CreateUser();
            user.Roles = new List<string> { Roles.GestorONG, Roles.Doador };

            var token = CreateGenerator().Generate(user);
            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

            var roles = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            Assert.Contains(Roles.GestorONG, roles);
            Assert.Contains(Roles.Doador, roles);
        }
    }
}
