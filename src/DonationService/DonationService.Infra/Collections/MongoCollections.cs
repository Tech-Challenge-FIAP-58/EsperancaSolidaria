using DonationService.Domain.Models;
using MongoDB.Driver;

namespace DonationService.Infra.Collections
{
	public class MongoCollections(
		IMongoDatabase database)
	{
		public IMongoCollection<Donation> Donation { get; } =
			database.GetCollection<Donation>("donations");

		public IMongoCollection<DonationLog> DonationLogs { get; } = 
			database.GetCollection<DonationLog>("donation_logs");
	}
}
