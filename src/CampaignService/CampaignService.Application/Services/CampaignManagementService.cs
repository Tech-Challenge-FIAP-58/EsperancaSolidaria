using CampaignService.Domain.Entities;
using CampaignService.Domain.Entities.DTOs;
using CampaignService.Domain.Interfaces;

namespace CampaignService.Application.Services
{
    public class CampaignManagementService : BaseService, ICampaignManagementService
    {
        public  async Task<ObjectReply<bool>> CancelCampaign(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ObjectReply<bool>> CreateCampaign(CreateCampaignDto dto)
        {
            var campaignCreated = new CampaignCreated();
            var campaign = campaignCreated.CreateCampaign(dto.Title, dto.Description, dto.StartDate, dto.EndDate, dto.FinancialTarget);

            return Success<bool>(message: $"Campanha #{campaign.Id} criada com sucesso");
        }

        public async Task<ObjectReply<bool>> UpdateCampaign(UpdateCampaignDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
