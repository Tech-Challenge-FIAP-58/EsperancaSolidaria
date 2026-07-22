using MongoDB.Bson.Serialization.Attributes;

namespace EsperancaSolidaria.Contracts.Entities
{
	public class ModelBase
	{
		[BsonId]
		public Guid Id { get; set; }
		public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
		public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
	}
}
