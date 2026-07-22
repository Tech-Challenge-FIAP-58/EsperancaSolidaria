using DonationService.Infra.Collections;
using DonationService.Infra.Indexes;

namespace DonationService.Infra.Bootstrap
{
	public sealed class MongoBootstrap(
		MongoCollectionInitializer collections,
		DonationIndexes indexes)
	{
		private readonly MongoCollectionInitializer _collections = collections;

		private readonly DonationIndexes _indexes = indexes;

		public async Task InitializeAsync()
		{
			await _collections.InitializeAsync();

			await _indexes.CreateAsync();
		}
	}
}
