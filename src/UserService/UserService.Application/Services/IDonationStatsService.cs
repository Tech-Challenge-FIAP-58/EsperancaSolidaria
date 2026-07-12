using EsperancaSolidaria.Contracts.Events;
using UserService.Application.Inputs;
using UserService.Application.Web;

namespace UserService.Application.Services
{
	public interface IDonationStatsService
	{
		Task RegisterDonation(Guid messageId, DonationReceivedEvent evt);

		Task<IApiResponse<UserDonationTotalDto>> GetTotal(Guid userId);
	}
}
