using EsperancaSolidaria.Contracts.Entities;

namespace DonationService.Domain.Models
{
	public class DonationLog : ModelBase
	{
		public Guid CampaignId { get; private set; }
		public string Message { get; private set; }

		public DonationLog(Guid campaignId, string message)
		{
			Id = Guid.NewGuid();
			CampaignId = campaignId;
			Message = message;
		}
	}
}
