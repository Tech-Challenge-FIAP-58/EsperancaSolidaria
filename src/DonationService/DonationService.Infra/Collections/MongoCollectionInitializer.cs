using MongoDB.Driver;

namespace DonationService.Infra.Collections
{
	public class MongoCollectionInitializer(
		IMongoDatabase database)
	{
		private readonly IMongoDatabase _database = database;

		public async Task InitializeAsync()
		{
			var collections =
				await _database
					.ListCollectionNames()
					.ToListAsync();

			if (!collections.Contains(CollectionsNames.DonationLogs))
			{
				await _database.CreateCollectionAsync(
					CollectionsNames.DonationLogs);
			}
		}
	}
}
