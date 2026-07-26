using System.ComponentModel.DataAnnotations;

namespace DonationService.WebApi.Settings
{
	public sealed class JwtSettings
	{
		[Required]
		public string Key { get; set; } = string.Empty;

		[Required]
		public string Issuer { get; set; } = string.Empty;

		[Required]
		public string Audience { get; set; } = string.Empty;
	}
}
