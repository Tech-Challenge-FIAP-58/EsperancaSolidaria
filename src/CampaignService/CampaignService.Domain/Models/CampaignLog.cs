using EsperancaSolidaria.Contracts.Entities;

namespace CampaignService.Domain.Models
{
	public class CampaignLog : ModelBase
	{
		public Guid CampaignId { get; private set; }
		public string Message { get; private set; }

		public CampaignLog(Guid campaignId, string message)
		{
			Id = Guid.NewGuid();
			CampaignId = campaignId;
			Message = message;
		}
	}
}
