using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DonationService.Application.Services;
using DonationService.Domain.Entities.DTOs;
using DonationService.Domain.Models;
using DonationService.Infra.Repositories.Interfaces;
using DonationService.Domain.Interfaces.MassTransit.Producer;
using EsperancaSolidaria.Contracts.Events;
using Moq;
using Xunit;

namespace DonationService.Test.Application
{
	public class CampaignDonationServiceTests
	{
		[Fact]
		public async Task CreateDonation_ValidDto_PublishesEventAndSavesDonation()
		{
			// Arrange
			var producerMock = new Mock<IDonationReceivedEventProducer>();
			var repoMock = new Mock<IDonationRepository>();
			var logRepoMock = new Mock<IDonationLogRepository>();

			var service = new CampaignDonationService(producerMock.Object, repoMock.Object, logRepoMock.Object);

			var dto = new CreateDonationDto(Guid.NewGuid(), Guid.NewGuid(), 50m);

			// Act
			var response = await service.CreateDonation(dto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.True(response.ResultValue);

			producerMock.Verify(p => p.Publish(It.Is<DonationReceivedEvent>(e =>
				e.DonorUserId == dto.DonorUserId && e.CampaignId == dto.CampaignId && e.Amount == dto.Amount)), Times.Once);

			repoMock.Verify(r => r.CreateDonation(It.IsAny<Donation>()), Times.Once);

			logRepoMock.Verify(l => l.WriteLog(It.IsAny<Guid>(), dto.CampaignId,
				It.Is<string>(s => s.Contains("Doação publicada") && s.Contains("Amount="))), Times.Once);
		}

		[Fact]
		public async Task GetDonationsByCampaignId_WhenNone_ReturnsNotFound()
		{
			// Arrange
			var producerMock = new Mock<IDonationReceivedEventProducer>();
			var repoMock = new Mock<IDonationRepository>();
			var logRepoMock = new Mock<IDonationLogRepository>();

			repoMock.Setup(r => r.GetDonations(It.IsAny<Guid>())).ReturnsAsync(new List<Donation>());

			var service = new CampaignDonationService(producerMock.Object, repoMock.Object, logRepoMock.Object);

			// Act
			var response = await service.GetDonationsByCampaignId(Guid.NewGuid());

			// Assert
			Assert.False(response.IsSuccess);
		}

		[Fact]
		public async Task GetDonationsByCampaignId_WhenHasDonations_ReturnsOk()
		{
			// Arrange
			var producerMock = new Mock<IDonationReceivedEventProducer>();
			var repoMock = new Mock<IDonationRepository>();
			var logRepoMock = new Mock<IDonationLogRepository>();

			var donation = new Donation(Guid.NewGuid(), Guid.NewGuid(), 20m);
			repoMock.Setup(r => r.GetDonations(donation.CampaignId)).ReturnsAsync(new List<Donation> { donation });

			var service = new CampaignDonationService(producerMock.Object, repoMock.Object, logRepoMock.Object);

			// Act
			var response = await service.GetDonationsByCampaignId(donation.CampaignId);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.NotNull(response.ResultValue);
			Assert.Single(response.ResultValue!);
		}

		[Fact]
		public async Task RejectDonation_CallsRepositoryAndWritesLog_ReturnsOk()
		{
			// Arrange
			var producerMock = new Mock<IDonationReceivedEventProducer>();
			var repoMock = new Mock<IDonationRepository>();
			var logRepoMock = new Mock<IDonationLogRepository>();

			var dto = new DonationService.Domain.Entities.DTOs.DonationRejectedDto(Guid.NewGuid(), Guid.NewGuid(), "motivo");

			var service = new CampaignDonationService(producerMock.Object, repoMock.Object, logRepoMock.Object);

			// Act
			var response = await service.RejectDonation(dto);

			// Assert
			Assert.True(response.IsSuccess);
			repoMock.Verify(r => r.DeactivateDonation(dto.DonationId), Times.Once);
			logRepoMock.Verify(l => l.WriteLog(dto.DonationId, dto.CampaignId, It.Is<string>(s => s.Contains("Doação cancelada/rejeitada"))), Times.Once);
		}
	}
}
