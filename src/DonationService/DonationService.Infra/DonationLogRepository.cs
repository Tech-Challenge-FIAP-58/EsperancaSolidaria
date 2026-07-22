using DonationService.Domain.Models;
using DonationService.Infra.Collections;
using DonationService.Infra.Repositories.Interfaces;

namespace DonationService.Infra
{
	public class DonationLogRepository(MongoCollections collections) : MongoRepository<DonationLog>(collections.DonationLogs), IDonationLogRepository
	{
		public async Task WriteLog(Guid donationId, Guid campaignId, string message)
		{
			await Add(new DonationLog(donationId, campaignId, message));
		}
	}
}
