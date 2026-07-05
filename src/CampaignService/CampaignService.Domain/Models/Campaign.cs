namespace CampaignService.Domain.Models
{
    public class Campaign(string title, string description, DateTime startDate, DateTime endDate, decimal financialTarget)
    {
        public Guid CampaignId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = title;
        public string Description { get; set; } = description;
        public DateTime StartDate { get; set; } = startDate;
        public DateTime EndDate { get; set; } = endDate;
        public decimal FinancialTarget { get; set; } = financialTarget;
        public bool IsActive { get; set; } = true;
        public decimal CollectedAmount { get; set; } = 0M;
    }
}
