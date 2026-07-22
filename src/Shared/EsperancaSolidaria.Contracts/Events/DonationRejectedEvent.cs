namespace EsperancaSolidaria.Contracts.Events
{
	public class DonationRejectedEvent
	{
		public Guid DonationId { get; init; }
		public Guid CampaignId { get; init; }
		public required string Message { get; init; }
	}
}
