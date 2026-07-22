using System;
using DonationService.Domain.Entities;
using Xunit;

namespace DonationService.Test.Domain
{
	public class DonationReceivedTests
	{
		[Fact]
		public void CreateDonation_WithPositiveAmount_ReturnsDonation()
		{
			// Arrange
			var donorUserId = Guid.NewGuid();
			var campaignId = Guid.NewGuid();
			decimal amount = 100m;
			var donationReceived = new DonationReceived();

			// Act
			var donation = donationReceived.CreateDonation(donorUserId, campaignId, amount);

			// Assert
			Assert.NotNull(donation);
			Assert.Equal(donorUserId, donation.DonorUserId);
			Assert.Equal(campaignId, donation.CampaignId);
			Assert.Equal(amount, donation.Amount);
			Assert.True(donation.IsActive);
			Assert.NotEqual(Guid.Empty, donation.Id);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-10)]
		public void CreateDonation_WithNonPositiveAmount_ThrowsArgumentException(decimal amount)
		{
			// Arrange
			var donorUserId = Guid.NewGuid();
			var campaignId = Guid.NewGuid();
			var donationReceived = new DonationReceived();

			// Act & Assert
			Assert.Throws<ArgumentException>(() => donationReceived.CreateDonation(donorUserId, campaignId, amount));
		}
	}
}
