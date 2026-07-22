using EsperancaSolidaria.Contracts.Events;

namespace CampaignService.Domain.Interfaces.MassTransit.Producer
{
	public interface IDonationRejectedEventProducer
	{
		Task Publish(DonationRejectedEvent message);
	}
}
