using CampaignService.Domain.Entities;
using CampaignService.Domain.Interfaces;

namespace CampaignService.Application.Services
{
    public class CampaignManagementService : BaseService, ICampaignManagementService
    {
        public  async Task<ObjectReply<bool>> CancelCampaign(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ObjectReply<bool>> CreateCampaign(string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget)
        {
            var campaignCreated = new CampaignCreated();
            var campaign = campaignCreated.CreateCampaign(title, description, startDate, endDate, financialTarget);

            return Success<bool>(message: $"Campanha #{campaign.Id} criada com sucesso");
        }

        public async Task<ObjectReply<bool>> UpdateCampaign(Guid id, string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget)
        {
            throw new NotImplementedException();
        }
    }
}
