namespace DonationService.Domain.Entities.DTOs
{
	public record DonationRejectedDto(Guid DonationId, Guid CampaignId, string Message);
}
