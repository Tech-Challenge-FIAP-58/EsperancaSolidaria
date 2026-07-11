using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserService.Application.Consumers;
using UserService.Application.Services;
using UserService.Domain.Events;

namespace UserService.Test
{
    public class DonationReceivedEventConsumerTests
    {
        [Fact]
        public async Task Consumer_ConsumesEvent_AndCallsService()
        {
            var service = new Mock<IDonationStatsService>();

            await using var provider = new ServiceCollection()
                .AddSingleton(service.Object)
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<DonationReceivedEventConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            try
            {
                var evt = new DonationReceivedEvent
                {
                    DonationId = Guid.NewGuid(),
                    DonorUserId = Guid.NewGuid(),
                    CampaignId = Guid.NewGuid(),
                    Amount = 100m,
                    OccurredAt = DateTimeOffset.UtcNow
                };

                await harness.Bus.Publish(evt);

                // O bus consumiu a mensagem...
                Assert.True(await harness.Consumed.Any<DonationReceivedEvent>());

                // ...e o consumer específico a processou...
                var consumerHarness = harness.GetConsumerHarness<DonationReceivedEventConsumer>();
                Assert.True(await consumerHarness.Consumed.Any<DonationReceivedEvent>());

                // ...delegando ao serviço com o mesmo DonationId.
                service.Verify(
                    s => s.RegisterDonation(It.Is<DonationReceivedEvent>(e => e.DonationId == evt.DonationId)),
                    Times.Once);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
