using System;
using Xunit;
using CampaignService.Domain.Entities;
using CampaignService.Domain.Models;

namespace CampaignService.Tests.Domain
{
	public class CampaignEntityTests
	{
		[Fact]
		public void CreateCampaign_ValidData_ShouldCreate()
		{
			var title = "Title";
			var description = "Desc";
			var start = DateTime.Now;
			var end = DateTime.Now.AddDays(10);
			var target = 1000m;

			var creator = new CampaignCreated();
			var campaign = creator.CreateCampaign(title, description, start, end, target);

			Assert.Equal(title, campaign.Title);
			Assert.Equal(description, campaign.Description);
			Assert.Equal(target, campaign.FinancialTarget);
			Assert.True(campaign.IsActive);
			Assert.Equal(0m, campaign.CollectedAmount);
		}

		[Fact]
		public void CreateCampaign_EndDateInPast_ShouldThrow()
		{
			var creator = new CampaignCreated();
			Assert.Throws<ArgumentException>(() => creator.CreateCampaign("t", "d", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-1), 100m));
		}

		[Fact]
		public void CreateCampaign_InvalidTarget_ShouldThrow()
		{
			var creator = new CampaignCreated();
			Assert.Throws<ArgumentException>(() => creator.CreateCampaign("t", "d", DateTime.Now, DateTime.Now.AddDays(1), 0m));
		}

		[Fact]
		public void UpdateCampaign_Valid_ShouldUpdate()
		{
			var campaign = new Campaign("a","b", DateTime.Now, DateTime.Now.AddDays(5), 100m);
			var updater = new CampaignUpdated(campaign);

			var newTitle = "new";
			updater.UpdateCampaign(newTitle, "desc", DateTime.Now, DateTime.Now.AddDays(7), 200m);

			Assert.Equal(newTitle, campaign.Title);
			Assert.Equal(200m, campaign.FinancialTarget);
		}

		[Fact]
		public void UpdateCampaign_InvalidEndDate_ShouldThrow()
		{
			var campaign = new Campaign("a","b", DateTime.Now, DateTime.Now.AddDays(5), 100m);
			var updater = new CampaignUpdated(campaign);
			Assert.Throws<ArgumentException>(() => updater.UpdateCampaign("t","d", DateTime.Now, DateTime.Now.AddDays(-1), 100m));
		}

		[Fact]
		public void AddDonation_Valid_ShouldIncreaseCollected()
		{
			var campaign = new Campaign("a","b", DateTime.Now, DateTime.Now.AddDays(5), 100m);
			var donation = new CampaignDonationUpdated(campaign);
			donation.AddDonation(50m);
			Assert.Equal(50m, campaign.CollectedAmount);
		}

		[Fact]
		public void AddDonation_InvalidAmount_ShouldThrow()
		{
			var campaign = new Campaign("a","b", DateTime.Now, DateTime.Now.AddDays(5), 100m);
			var donation = new CampaignDonationUpdated(campaign);
			Assert.Throws<ArgumentException>(() => donation.AddDonation(0m));
		}

		[Fact]
		public void AddDonation_InactiveCampaign_ShouldThrow()
		{
			var campaign = new Campaign("a","b", DateTime.Now, DateTime.Now.AddDays(5), 100m);
			campaign.CancelCampaign();
			var donation = new CampaignDonationUpdated(campaign);
			Assert.Throws<InvalidOperationException>(() => donation.AddDonation(10m));
		}
	}
}
