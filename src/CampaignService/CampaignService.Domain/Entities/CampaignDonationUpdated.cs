using CampaignService.Domain.Models;

namespace CampaignService.Domain.Entities
{
    public class CampaignDonationUpdated(Campaign campaign)
    {
        private readonly Campaign _campaign = campaign;

        public void AddDonation(decimal amount)
        {
            if (!_campaign.IsActive)
            {
                throw new InvalidOperationException("Não é possível adicionar uma doação a uma campanha inativa");
			}
            if (amount <= 0)
            {
                throw new ArgumentException("O valor da doação deve ser maior que zero");
            }

            _campaign.AddCollectedAmount(amount);
        }
    }
}
