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

			var messageId = context.MessageId
				?? throw new InvalidOperationException("Mensagem sem MessageId; não é possível garantir idempotência.");

			logger.LogInformation(
				"Evento DonationReceived recebido: mensagem {MessageId}, usuário {UserId}, valor {Amount}.",
				messageId, message.DonorUserId, message.Amount);

			await service.RegisterDonation(messageId, message);
		}
	}
}
