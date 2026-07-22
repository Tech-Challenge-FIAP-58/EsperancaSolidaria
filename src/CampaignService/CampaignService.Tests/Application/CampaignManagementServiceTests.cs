using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CampaignService.Application.Services;
using CampaignService.Domain.Entities.DTOs;
using CampaignService.Domain.Models;
using CampaignService.Infra.Repositories.Interfaces;
using CampaignService.Domain.Interfaces.MassTransit.Producer;

namespace CampaignService.Tests.Application
{
	public class CampaignManagementServiceTests
	{
		[Fact]
		public async Task Create_ShouldReturnCreatedAndCallRepository()
		{
			var repoMock = new Mock<ICampaignRepository>();
			var logMock = new Mock<ICampaignLogRepository>();
			var producerMock = new Mock<IDonationRejectedEventProducer>();

			var service = new CampaignManagementService(repoMock.Object, logMock.Object, producerMock.Object);

			var dto = new CreateCampaignDto("t","d", DateTime.Now, DateTime.Now.AddDays(5), 100m);
			var response = await service.Create(dto);

			Assert.True(response.IsSuccess);
			Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
			repoMock.Verify(r => r.CreateCampaign(It.IsAny<Campaign>()), Times.Once);
		}

		[Fact]
		public async Task GetById_NotFound_ShouldReturnNotFound()
		{
			var repoMock = new Mock<ICampaignRepository>();
			repoMock.Setup(r => r.GetCampaignById(It.IsAny<Guid>())).ReturnsAsync((Campaign?)null);

			var logMock = new Mock<ICampaignLogRepository>();
			var producerMock = new Mock<IDonationRejectedEventProducer>();

			var service = new CampaignManagementService(repoMock.Object, logMock.Object, producerMock.Object);

			var response = await service.GetById(Guid.NewGuid());

			Assert.False(response.IsSuccess);
			Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task AddDonation_NotFound_ShouldReturnNotFound()
		{
			var repoMock = new Mock<ICampaignRepository>();
			repoMock.Setup(r => r.GetCampaignById(It.IsAny<Guid>())).ReturnsAsync((Campaign?)null);

			var logMock = new Mock<ICampaignLogRepository>();
			var producerMock = new Mock<IDonationRejectedEventProducer>();

			var service = new CampaignManagementService(repoMock.Object, logMock.Object, producerMock.Object);
			await service.AddDonation(new AddDonationDto(Guid.NewGuid(), Guid.NewGuid(), 10m));
		}

		[Fact]
		public async Task AddDonation_Valid_ShouldUpdateRepository()
		{
			var campaign = new Campaign("a","b", DateTime.Now, DateTime.Now.AddDays(5), 100m);
			var donationId = Guid.NewGuid();

			var repoMock = new Mock<ICampaignRepository>();
			repoMock.Setup(r => r.GetCampaignById(It.IsAny<Guid>())).ReturnsAsync(campaign);

			var logMock = new Mock<ICampaignLogRepository>();
			var producerMock = new Mock<IDonationRejectedEventProducer>();

			var service = new CampaignManagementService(repoMock.Object, logMock.Object, producerMock.Object);
			await service.AddDonation(new AddDonationDto(campaign.Id, donationId, 25m));

			repoMock.Verify(r => r.UpdateCampaign(It.Is<Campaign>(c => c.CollectedAmount == 25m)), Times.Once);
		}

		[Fact]
		public async Task CancelCampaign_NotFound_ShouldReturnNotFound()
		{
			var repoMock = new Mock<ICampaignRepository>();
			repoMock.Setup(r => r.GetCampaignById(It.IsAny<Guid>())).ReturnsAsync((Campaign?)null);

			var logMock = new Mock<ICampaignLogRepository>();
			var producerMock = new Mock<IDonationRejectedEventProducer>();

			var service = new CampaignManagementService(repoMock.Object, logMock.Object, producerMock.Object);

			var response = await service.CancelCampaign(Guid.NewGuid());

			Assert.False(response.IsSuccess);
			Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task CancelCampaign_Valid_ShouldUpdateRepository()
		{
			var campaign = new Campaign("a","b", DateTime.Now, DateTime.Now.AddDays(5), 100m);
			var repoMock = new Mock<ICampaignRepository>();
			repoMock.Setup(r => r.GetCampaignById(It.IsAny<Guid>())).ReturnsAsync(campaign);

			var logMock = new Mock<ICampaignLogRepository>();
			var producerMock = new Mock<IDonationRejectedEventProducer>();

			var service = new CampaignManagementService(repoMock.Object, logMock.Object, producerMock.Object);

			var response = await service.CancelCampaign(campaign.Id);

			Assert.True(response.IsSuccess);
			Assert.False(campaign.IsActive);
			repoMock.Verify(r => r.UpdateCampaign(It.Is<Campaign>(c => !c.IsActive)), Times.Once);
		}

		[Fact]
		public async Task Update_InvalidData_ShouldReturnBadRequest()
		{
			var campaign = new Campaign("a","b", DateTime.Now, DateTime.Now.AddDays(5), 100m);
			var repoMock = new Mock<ICampaignRepository>();
			repoMock.Setup(r => r.GetCampaignById(It.IsAny<Guid>())).ReturnsAsync(campaign);

			var logMock = new Mock<ICampaignLogRepository>();
			var producerMock = new Mock<IDonationRejectedEventProducer>();

			var service = new CampaignManagementService(repoMock.Object, logMock.Object, producerMock.Object);

			var dto = new UpdateCampaignDto(campaign.Id, "t", "d", DateTime.Now, DateTime.Now.AddDays(-1), 100m);
			var response = await service.Update(dto);

			Assert.False(response.IsSuccess);
			Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
}
