using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UserService.Application.Services;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Models;

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
            DonationId = Guid.NewGuid(),
            DonorUserId = Guid.NewGuid(),
            CampaignId = Guid.NewGuid(),
            Amount = 150.50m,
            OccurredAt = DateTimeOffset.UtcNow
        };

        [Fact]
        public async Task RegisterDonation_MapsFields_UsesDonationIdAsId_AndAdds()
        {
            var evt = NewEvent();
            UserDonationStat? saved = null;
            _repo.Setup(r => r.Add(It.IsAny<UserDonationStat>()))
                .Callback<UserDonationStat>(s => saved = s)
                .ReturnsAsync(true);

            await _service.RegisterDonation(evt);

            Assert.NotNull(saved);
            Assert.Equal(evt.DonationId, saved!.Guid);   // _id == DonationId (idempotência)
            Assert.Equal(evt.DonorUserId, saved.UserId);
            Assert.Equal(evt.CampaignId, saved.CampaignId);
            Assert.Equal(evt.Amount, saved.Amount);
            Assert.Equal(evt.OccurredAt, saved.OccurredAt);
            _repo.Verify(r => r.Add(It.IsAny<UserDonationStat>()), Times.Once);
        }

        [Fact]
        public async Task RegisterDonation_WhenDuplicate_DoesNotThrow()
        {
            var evt = NewEvent();
            _repo.Setup(r => r.Add(It.IsAny<UserDonationStat>())).ReturnsAsync(false);

            var ex = await Record.ExceptionAsync(() => _service.RegisterDonation(evt));

            Assert.Null(ex);
            _repo.Verify(r => r.Add(It.IsAny<UserDonationStat>()), Times.Once);
        }
    }
}
