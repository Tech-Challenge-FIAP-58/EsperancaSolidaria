using EsperancaSolidaria.Contracts.Entities;

namespace CampaignService.Domain.Models
{
    public class Campaign : ModelBase
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public decimal FinancialTarget { get; private set; }
        public bool IsActive { get; private set; }
        public decimal CollectedAmount { get; private set; }

        public Campaign(string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            FinancialTarget = financialTarget;
            IsActive = true;
            CollectedAmount = 0M;
        }

        public void UpdateCampaign(string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            FinancialTarget = financialTarget;
        }

        public void CancelCampaign()
        {
            IsActive = false;
        }

        public void AddCollectedAmount(decimal amount)
        {
            CollectedAmount += amount;
        }
    }
}
