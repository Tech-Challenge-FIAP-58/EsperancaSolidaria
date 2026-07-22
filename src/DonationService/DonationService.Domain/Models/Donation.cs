using EsperancaSolidaria.Contracts.Entities;

namespace DonationService.Domain.Models
{
	public class Donation : ModelBase
	{
		public Guid DonorUserId { get; set; }
		public Guid CampaignId { get; set; }
		public decimal Amount { get; set; }

		public Donation(Guid donorUserId, Guid campaignId, decimal amount)
		{
			Id = Guid.NewGuid();
			DonorUserId = donorUserId;
			CampaignId = campaignId;
			Amount = amount;
		}
	}
}
