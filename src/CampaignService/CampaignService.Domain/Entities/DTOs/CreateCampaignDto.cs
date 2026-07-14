
namespace CampaignService.Domain.Entities.DTOs
{
	public record CreateCampaignDto(string Title, string Description, DateTime StartDate, DateTime EndDate, decimal FinancialTarget);
}
