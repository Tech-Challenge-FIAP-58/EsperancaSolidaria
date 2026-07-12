using EsperancaSolidaria.Contracts.Events;
using Microsoft.Extensions.Logging;
using UserService.Application.Inputs;
using UserService.Application.Web;
using UserService.Domain.Interfaces.Repository;

namespace UserService.Application.Services
{
	public class DonationStatsService(
		IUserStatisticsRepository repository,
		ILogger<DonationStatsService> logger) : BaseService, IDonationStatsService
	{
		public async Task RegisterDonation(Guid messageId, DonationReceivedEvent evt)
		{
			var applied = await repository.RegisterDonation(messageId, evt.DonorUserId, evt.Amount);

			if (applied)
			{
				logger.LogInformation(
					"Mensagem {MessageId} somada ao total do usuário {UserId}: +{Amount}.",
					messageId, evt.DonorUserId, evt.Amount);
			}
			else
			{
				logger.LogInformation(
					"Mensagem {MessageId} já processada anteriormente; ignorada.",
					messageId);
			}
		}

		public async Task<IApiResponse<UserDonationTotalDto>> GetTotal(Guid userId)
		{
			var total = await repository.GetTotalByUser(userId);
			return Ok(new UserDonationTotalDto(userId, total));
		}
	}
}
