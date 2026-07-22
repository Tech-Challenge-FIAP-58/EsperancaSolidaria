namespace DonationService.Infra.Repositories.Interfaces
{
	public interface IDonationLogRepository
	{
		Task WriteLog(Guid donationId, Guid campaignId, string message);
	}
}
