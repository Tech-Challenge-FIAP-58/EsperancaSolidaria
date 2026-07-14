using CampaignService.Domain.Entities.DTOs;
using EsperancaSolidaria.Contracts.Entities.Web;

namespace CampaignService.Domain.Interfaces
{
    public interface ICampaignManagementService
    {
        Task<ObjectReply<bool>> CreateCampaign(CreateCampaignDto dto);
        Task<ObjectReply<bool>> UpdateCampaign(UpdateCampaignDto dto);
        Task<ObjectReply<bool>> CancelCampaign(Guid id);
    }
}
