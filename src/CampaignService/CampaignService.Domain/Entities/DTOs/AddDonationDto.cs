
namespace CampaignService.Domain.Entities.DTOs
{
	public sealed record AddDonationDto(Guid CampaignId, Guid DonationId, decimal Amount);
}
