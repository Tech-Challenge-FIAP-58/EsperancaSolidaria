using System.ComponentModel.DataAnnotations;

namespace DonationService.Infra.Configurations
{
	public class MongoConfig
	{
		[Required]
		public string ConnectionString { get; set; } = string.Empty;
		[Required]
		public string Database { get; set; } = string.Empty;
	}
}
