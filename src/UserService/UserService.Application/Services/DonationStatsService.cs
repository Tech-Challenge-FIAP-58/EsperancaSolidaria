using EsperancaSolidaria.Contracts.Events;
using Microsoft.Extensions.Logging;
using UserService.Domain.Interfaces.Repository;

namespace UserService.Application.Services
{
	public class DonationStatsService(
		IUserStatisticsRepository repository,
		ILogger<DonationStatsService> logger) : IDonationStatsService
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
	}
}
