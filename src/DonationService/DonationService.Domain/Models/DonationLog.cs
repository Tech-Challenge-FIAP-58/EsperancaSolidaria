using EsperancaSolidaria.Contracts.Entities;

namespace DonationService.Domain.Models
{
	public class DonationLog : ModelBase
	{
		public Guid DonationId { get; private set; }
		public Guid CampaignId { get; private set; }
		public string Message { get; private set; }

		public DonationLog(Guid donationId, Guid campaignId, string message)
		{
			Id = Guid.NewGuid();
			DonationId = donationId;
			CampaignId = campaignId;
			Message = message;
		}
	}
}
