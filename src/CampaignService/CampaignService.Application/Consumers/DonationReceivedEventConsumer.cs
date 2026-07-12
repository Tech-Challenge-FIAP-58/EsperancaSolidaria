using EsperancaSolidaria.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CampaignService.Application.Consumers
{
	public class DonationReceivedEventConsumer(
		ILogger<DonationReceivedEventConsumer> logger) : IConsumer<DonationReceivedEvent>
	{
		public Task Consume(ConsumeContext<DonationReceivedEvent> context)
		{
			var message = context.Message;

			logger.LogInformation(
				"Evento DonationReceived recebido: mensagem {MessageId}, doação {DonationId}, campanha {CampaignId}, usuário {UserId}, valor {Amount}.",
				context.MessageId, message.DonationId, message.CampaignId, message.DonorUserId, message.Amount);

			return Task.CompletedTask;
		}
	}
}
