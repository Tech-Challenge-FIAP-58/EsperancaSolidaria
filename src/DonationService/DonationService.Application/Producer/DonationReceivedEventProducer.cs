using DonationService.Domain.Events;
using DonationService.Domain.Interfaces.MassTransit.Producer;
using MassTransit;

namespace DonationService.Application.Producer
{
	public class DonationReceivedEventProducer(ISendEndpointProvider sendEndpointProvider) : IDonationReceivedEventProducer
	{
		public async Task Send(DonationReceivedEvent message)
		{
			var endpoint = await sendEndpointProvider
				.GetSendEndpoint(new Uri("queue:DonationReceivedEvent"));

			await endpoint.Send(message);
		}
	}
}
