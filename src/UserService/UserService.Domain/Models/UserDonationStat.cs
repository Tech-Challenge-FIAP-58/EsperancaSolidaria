using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Domain.Models
{
	/// <summary>
	/// Registro imutável (append-only) de uma doação, gravado na coleção user_statistics.
	/// Cada doação vira um documento; a "quantidade de doações" de um usuário é derivada
	/// por consulta (count), não por campo denormalizado.
	/// O <see cref="EntityBase.Guid"/> (_id) recebe o DonationId: reentrega da mesma
	/// mensagem colide no _id e é ignorada (idempotência).
	/// </summary>
	public class UserDonationStat : EntityBase
	{
		public Guid UserId { get; set; }
		public Guid CampaignId { get; set; }

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal Amount { get; set; }

		public DateTimeOffset OccurredAt { get; set; }
	}
}
