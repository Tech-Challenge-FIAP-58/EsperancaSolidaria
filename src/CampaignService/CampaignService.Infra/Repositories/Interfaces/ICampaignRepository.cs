using CampaignService.Domain.Models;

namespace CampaignService.Infra.Repositories.Interfaces
{
    public interface ICampaignRepository
    {
        Task<Campaign?> GetCampaignById(Guid guid);
        Task<IEnumerable<Campaign>> GetAllCampaigns();
        Task CreateCampaign(Campaign campaign);
        Task UpdateCampaign(Campaign campaign);
	}
}
