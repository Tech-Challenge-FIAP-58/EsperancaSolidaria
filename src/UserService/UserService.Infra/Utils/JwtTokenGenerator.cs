using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Interfaces.Utils;
using UserService.Domain.Models;
using UserService.Infra.Configurations;

namespace UserService.Infra.Utils
{
    public class JwtTokenGenerator(IOptions<JwtSettings> options) : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings = options.Value;

        public string Generate(User user)
        {
            if (string.IsNullOrWhiteSpace(_settings.Key))
                throw new InvalidOperationException("JWT Key não configurada (Jwt:Key).");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Guid.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Name),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in (user.Roles ?? Enumerable.Empty<string>()).Distinct())
                claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_settings.Expiration),
                SigningCredentials = credentials,
                Issuer = _settings.Issuer,
                Audience = _settings.Audience
            };

            var handler = new JsonWebTokenHandler();
            return handler.CreateToken(tokenDescriptor);
        }
    }
}
