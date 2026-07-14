namespace CampaignService.Domain.Entities.DTOs
{
	public record UpdateCampaignDto(Guid Id, string Title, string Description, DateTime StartDate, DateTime EndDate, decimal FinancialTarget);
}
