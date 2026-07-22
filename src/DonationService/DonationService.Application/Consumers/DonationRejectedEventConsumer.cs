using DonationService.Domain.Entities.DTOs;
using DonationService.Domain.Interfaces;
using EsperancaSolidaria.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DonationService.Application.Consumers
{
	public class DonationRejectedEventConsumer(
		ILogger<DonationRejectedEventConsumer> logger, ICampaignDonationServiceDonationService campaignDonationServiceDonationService) : IConsumer<DonationRejectedEvent>
	{
		public async Task Consume(ConsumeContext<DonationRejectedEvent> context)
		{
			var message = context.Message;

			logger.LogInformation("Evento de cancelamento de doação recebido");

			await campaignDonationServiceDonationService.RejectDonation(new 
				DonationRejectedDto(message.DonationId, message.CampaignId, message.Message));
		}
	}
}
