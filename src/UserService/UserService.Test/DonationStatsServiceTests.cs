using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using EsperancaSolidaria.Contracts.Events;
using UserService.Application.Services;
using UserService.Domain.Interfaces.Repository;

namespace UserService.Test
{
    public class DonationStatsServiceTests
    {
        private readonly Mock<IUserStatisticsRepository> _repo = new();
        private readonly DonationStatsService _service;

        public DonationStatsServiceTests()
        {
            _service = new DonationStatsService(_repo.Object, NullLogger<DonationStatsService>.Instance);
        }

        private static DonationReceivedEvent NewEvent() => new()
        {
            DonorUserId = Guid.NewGuid(),
            Amount = 150.50m
        };

        [Fact]
        public async Task RegisterDonation_ForwardsMessageIdUserAndAmount()
        {
            var messageId = Guid.NewGuid();
            var evt = NewEvent();
            _repo.Setup(r => r.RegisterDonation(messageId, evt.DonorUserId, evt.Amount)).ReturnsAsync(true);

            await _service.RegisterDonation(messageId, evt);

            _repo.Verify(r => r.RegisterDonation(messageId, evt.DonorUserId, evt.Amount), Times.Once);
        }

        [Fact]
        public async Task RegisterDonation_WhenDuplicate_DoesNotThrow()
        {
            var evt = NewEvent();
            _repo.Setup(r => r.RegisterDonation(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>())).ReturnsAsync(false);

            var ex = await Record.ExceptionAsync(() => _service.RegisterDonation(Guid.NewGuid(), evt));

            Assert.Null(ex);
        }
    }
}
