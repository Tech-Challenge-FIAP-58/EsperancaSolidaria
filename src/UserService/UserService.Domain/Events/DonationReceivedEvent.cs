namespace UserService.Domain.Events
{
	public class DonationReceivedEvent
	{
		public Guid DonationId { get; init; }
		public Guid DonorUserId { get; init; }
		public Guid CampaignId { get; init; }
		public decimal Amount { get; init; }
		public DateTimeOffset OccurredAt { get; init; }
	}
}
