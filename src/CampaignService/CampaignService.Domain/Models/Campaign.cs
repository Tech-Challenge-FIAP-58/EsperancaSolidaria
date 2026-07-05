namespace CampaignService.Domain.Models
{
    public class Campaign
    {
        public Guid CampaignId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal FinancialTarget { get; set; }
        public bool IsActive { get; set; }
        public decimal CollectedAmount { get; set; }

        public Campaign(string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget)
        {
            CampaignId = Guid.NewGuid();
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
    }
}
