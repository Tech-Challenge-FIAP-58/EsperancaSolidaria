using CampaignService.Domain.Entities;
using CampaignService.Domain.Entities.DTOs;

namespace CampaignService.Domain.Interfaces
{
    public interface ICampaignManagementService
    {
        Task<ObjectReply<bool>> CreateCampaign(CreateCampaignDto dto);
        Task<ObjectReply<bool>> UpdateCampaign(UpdateCampaignDto dto);
        Task<ObjectReply<bool>> CancelCampaign(Guid id);
    }
}
