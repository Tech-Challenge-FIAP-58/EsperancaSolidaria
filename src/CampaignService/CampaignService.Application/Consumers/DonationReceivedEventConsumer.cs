using CampaignService.Domain.Entities.DTOs;
using CampaignService.Domain.Interfaces;
using EsperancaSolidaria.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CampaignService.Application.Consumers
{
	public class DonationReceivedEventConsumer(
		ILogger<DonationReceivedEventConsumer> logger, ICampaignManagementService campaignManagementService) : IConsumer<DonationReceivedEvent>
	{
		public async Task Consume(ConsumeContext<DonationReceivedEvent> context)
		{
			var message = context.Message;

			logger.LogInformation(
				"Evento DonationReceived recebido: mensagem {MessageId}, doação {DonationId}, campanha {CampaignId}, usuário {UserId}, valor {Amount}.",
				context.MessageId, message.DonationId, message.CampaignId, message.DonorUserId, message.Amount);

			await campaignManagementService.AddDonation(new AddDonationDto(message.CampaignId, message.DonationId, message.Amount));
		}
	}
}
