using DonationService.Domain.Models;

namespace DonationService.Domain.Entities
{
	public class DonationReceived
	{
		public Donation CreateDonation(Guid donorUserId, Guid campaignId, decimal amount)
		{
			if (amount <= 0)
			{
				throw new ArgumentException("Amount must be a positive value.", nameof(amount));
			}

			return new Donation(donorUserId, campaignId, amount);
		}
	}
}
