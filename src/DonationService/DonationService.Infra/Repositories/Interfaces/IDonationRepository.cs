using DonationService.Domain.Models;

namespace DonationService.Infra.Repositories.Interfaces
{
	public interface IDonationRepository
	{
		Task<IEnumerable<Donation>> GetDonations(Guid campaignId);
		Task CreateDonation(Donation donation);
		Task DeactivateDonation(Guid donationId);
	}
}
