using System.ComponentModel.DataAnnotations;

namespace CampaignService.Infra.Configurations
{
    public sealed class MongoConfig
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;
        [Required]
        public string Database { get; set; } = string.Empty;
    }
}
