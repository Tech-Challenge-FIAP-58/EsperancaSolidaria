using CampaignService.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CampaignService.Application.Consumers
{
	public class DonationReceivedEventConsumer(
		ILogger<DonationReceivedEventConsumer> logger) : IConsumer<DonationReceivedEvent>
	{
		public Task Consume(ConsumeContext<DonationReceivedEvent> context)
		{
			throw new NotImplementedException();
		}
	}
}
