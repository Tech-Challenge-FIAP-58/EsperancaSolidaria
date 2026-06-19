using DonationService.Domain.Events;

namespace DonationService.Domain.Interfaces.MassTransit.Producer
{
	public interface IDonationReceivedEventProducer
	{
		Task Send(DonationReceivedEvent message);
	}
}
