using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Domain.Models
{
	public class UserDonationStat : EntityBase
	{
		public Guid UserId { get; set; }
		public Guid CampaignId { get; set; }

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal Amount { get; set; }

		public DateTimeOffset OccurredAt { get; set; }
	}
}
