using MassTransit;
using Microsoft.Extensions.Logging;
using UserService.Domain.Events;

namespace UserService.Application.Consumers
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
