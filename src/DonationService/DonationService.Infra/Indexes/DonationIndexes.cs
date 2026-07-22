using DonationService.Domain.Models;
using DonationService.Infra.Collections;
using MongoDB.Driver;

namespace DonationService.Infra.Indexes
{
	public class DonationIndexes(
		IMongoDatabase database)
	{
		private readonly IMongoDatabase _database = database;

		public async Task CreateAsync()
		{
			var collection = _database.
				GetCollection<DonationLog>(CollectionsNames.DonationLogs);
		}
	}
}
