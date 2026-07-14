using DonationService.Domain.Interfaces.MassTransit.Producer;
using EsperancaSolidaria.Contracts.Events;
using MassTransit;

namespace DonationService.Application.Producer
{
	// Publica em fan-out: o evento é entregue a todos os serviços inscritos
	// (UserService e CampaignService), cada um na sua própria fila.
	public class DonationReceivedEventProducer(IPublishEndpoint publishEndpoint) : IDonationReceivedEventProducer
	{
		public Task Publish(DonationReceivedEvent message) => publishEndpoint.Publish(message);
	}
}
