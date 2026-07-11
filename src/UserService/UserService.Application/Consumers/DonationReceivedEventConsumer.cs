using MassTransit;
using Microsoft.Extensions.Logging;
using UserService.Application.Services;
using UserService.Domain.Events;

namespace UserService.Application.Consumers
{
	public class DonationReceivedEventConsumer(
		IDonationStatsService service,
		ILogger<DonationReceivedEventConsumer> logger) : IConsumer<DonationReceivedEvent>
	{
		public async Task Consume(ConsumeContext<DonationReceivedEvent> context)
		{
			var message = context.Message;

			logger.LogInformation(
				"Evento DonationReceived recebido: doação {DonationId} do usuário {UserId}.",
				message.DonationId, message.DonorUserId);

			await service.RegisterDonation(message);
		}
	}
}
