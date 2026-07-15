using System.ComponentModel.DataAnnotations;

namespace DonationService.WebApi.Settings
{
	public class RabbitMqSettings
	{
		[Required]
		public required string Host { get; set; }
		[Required]
		public required string VirtualHost { get; set; }
		[Required]
		public required ushort Port { get; set; }
		[Required]
		public required string Username { get; set; }
		[Required]
		public required string Password { get; set; }
	}
}
