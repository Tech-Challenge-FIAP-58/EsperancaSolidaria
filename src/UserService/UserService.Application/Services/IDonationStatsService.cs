using UserService.Domain.Events;

namespace UserService.Application.Services
{
	public interface IDonationStatsService
	{
		Task RegisterDonation(Guid messageId, DonationReceivedEvent evt);
	}
}
