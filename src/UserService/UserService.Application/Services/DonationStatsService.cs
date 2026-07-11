using Microsoft.Extensions.Logging;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Models;

namespace UserService.Application.Services
{
	public class DonationStatsService(
		IUserStatisticsRepository repository,
		ILogger<DonationStatsService> logger) : IDonationStatsService
	{
		public async Task RegisterDonation(DonationReceivedEvent evt)
		{
			// DonationId vira o _id: garante idempotência contra reentrega do bus.
			var stat = new UserDonationStat
			{
				Guid = evt.DonationId,
				UserId = evt.DonorUserId,
				CampaignId = evt.CampaignId,
				Amount = evt.Amount,
				OccurredAt = evt.OccurredAt
			};

			var registered = await repository.Add(stat);

			if (registered)
			{
				logger.LogInformation(
					"Doação {DonationId} registrada para o usuário {UserId} (campanha {CampaignId}, valor {Amount}).",
					evt.DonationId, evt.DonorUserId, evt.CampaignId, evt.Amount);
			}
			else
			{
				logger.LogInformation(
					"Doação {DonationId} já processada anteriormente; duplicata ignorada.",
					evt.DonationId);
			}
		}
	}
}
