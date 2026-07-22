namespace DonationService.Domain.Entities.DTOs
{
	public record CreateDonationDto(Guid DonorUserId, Guid CampaignId, decimal Amount);
}
