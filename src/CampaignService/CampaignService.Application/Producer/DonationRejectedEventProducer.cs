using CampaignService.Domain.Interfaces.MassTransit.Producer;
using EsperancaSolidaria.Contracts.Events;
using MassTransit;

namespace CampaignService.Application.Producer
{
	public class DonationRejectedEventProducer(IPublishEndpoint publishEndpoint) : IDonationRejectedEventProducer
	{
		public Task Publish(DonationRejectedEvent message) => publishEndpoint.Publish(message);
	}
}
