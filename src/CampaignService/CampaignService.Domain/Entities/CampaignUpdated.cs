using CampaignService.Domain.Models;

namespace CampaignService.Domain.Entities
{
    public class CampaignUpdated(Campaign campaign)
    {
        private readonly Campaign _campaign = campaign;

        public void UpdateCampaign(string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget)
        {
            if (endDate < DateTime.Now)
            {
                throw new ArgumentException("A data de término não pode estar no passado");
            }
            if (financialTarget <= 0)
            {
                throw new ArgumentException("O valor alvo financeiro deve ser maior que zero");
            }

            _campaign.UpdateCampaign(title, description, startDate, endDate, financialTarget);
        }

        public void CancelCampaign()
        {
            _campaign.CancelCampaign();
        }
    }
}
