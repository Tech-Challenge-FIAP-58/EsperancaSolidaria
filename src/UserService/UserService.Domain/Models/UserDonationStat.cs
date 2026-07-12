using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Domain.Models
{
	public class UserDonationStat : EntityBase
	{
		public Guid UserId { get; set; }

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal TotalDonated { get; set; }
	}
}
