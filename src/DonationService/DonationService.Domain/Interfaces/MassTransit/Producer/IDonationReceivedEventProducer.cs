using EsperancaSolidaria.Contracts.Events;

namespace DonationService.Domain.Interfaces.MassTransit.Producer
{
	public interface IDonationReceivedEventProducer
	{
		Task Publish(DonationReceivedEvent message);
	}
}
