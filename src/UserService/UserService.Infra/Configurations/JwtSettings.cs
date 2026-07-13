using System.ComponentModel.DataAnnotations;

namespace UserService.Infra.Configurations
{
    public sealed class JwtSettings
    {
        [Required]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Expiration { get; set; }
    }
}
