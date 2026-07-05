using CampaignService.Domain.Entities;

namespace CampaignService.Domain.Interfaces
{
    public interface ICampaignManagementService
    {
        Task<ObjectReply<bool>> CreateCampaign(string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget);
        Task<ObjectReply<bool>> UpdateCampaign(Guid id, string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget);
        Task<ObjectReply<bool>> CancelCampaign(Guid id);
    }
}
