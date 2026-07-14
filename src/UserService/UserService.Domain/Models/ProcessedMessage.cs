using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Domain.Models
{
	public class ProcessedMessage
	{
		[BsonId]
		public Guid MessageId { get; set; }

		public DateTime ProcessedAt { get; set; }
	}
}
