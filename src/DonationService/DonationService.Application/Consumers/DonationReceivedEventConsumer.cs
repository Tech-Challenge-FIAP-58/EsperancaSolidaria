using EsperancaSolidaria.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DonationService.Application.Consumers
{
	public class DonationReceivedEventConsumer(
		ILogger<DonationReceivedEventConsumer> logger) : IConsumer<DonationReceivedEvent>
	{
		public async Task Consume(ConsumeContext<DonationReceivedEvent> context)
		{
			var message = context.Message;
		}
	}
}
