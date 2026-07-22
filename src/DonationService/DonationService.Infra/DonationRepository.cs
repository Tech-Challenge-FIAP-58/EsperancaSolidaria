using DonationService.Domain.Models;
using DonationService.Infra.Collections;
using DonationService.Infra.Repositories.Interfaces;

namespace DonationService.Infra
{
	public class DonationRepository(MongoCollections collections) : MongoRepository<Donation>(collections.Donation), IDonationRepository
	{
		public async Task CreateDonation(Donation donation)
		{
			await Add(donation);
		}

		public async Task<IEnumerable<Donation>> GetDonations(Guid campaignId)
		{
			var donations = await GetAll();
			return donations.ToList().FindAll(d => d.CampaignId == campaignId);
		}
	}
}
