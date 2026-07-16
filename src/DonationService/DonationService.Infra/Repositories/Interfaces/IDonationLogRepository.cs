namespace DonationService.Infra.Repositories.Interfaces
{
	public interface IDonationLogRepository
	{
		Task WriteLog(Guid campaignId, string message);
	}
}
