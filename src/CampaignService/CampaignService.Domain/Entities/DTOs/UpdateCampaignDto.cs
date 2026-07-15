namespace CampaignService.Domain.Entities.DTOs
{
	public sealed record UpdateCampaignDto(
		Guid Id, 
		string Title, 
		string Description,
		DateTime StartDate, 
		DateTime EndDate, 
		decimal FinancialTarget);
}
