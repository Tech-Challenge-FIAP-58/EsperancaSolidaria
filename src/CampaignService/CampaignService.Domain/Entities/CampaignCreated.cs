using CampaignService.Domain.Models;

namespace CampaignService.Domain.Entities
{
    public class CampaignCreated
    {
        public Campaign CreateCampaign(string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget)
        {
            if (endDate < DateTime.Now)
            {
                throw new ArgumentException("A data de término não pode estar no passado");
            }
            if (financialTarget <= 0)
            {
                throw new ArgumentException("O valor alvo financeiro deve ser maior que zero");
            }

            var campaign = new Campaign(title, description, startDate, endDate, financialTarget);
            return campaign;
        }
    }
}
