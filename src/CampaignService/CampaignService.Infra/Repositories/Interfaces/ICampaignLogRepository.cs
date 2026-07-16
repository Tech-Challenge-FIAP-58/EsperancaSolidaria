namespace CampaignService.Infra.Repositories.Interfaces
{
	public interface ICampaignLogRepository
	{
		Task WriteLog(Guid campaignId, string message);
	}
}
