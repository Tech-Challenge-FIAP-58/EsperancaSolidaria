namespace UserService.Domain.Events
{
	public class DonationReceivedEvent
	{
		public Guid DonorUserId { get; init; }
		public decimal Amount { get; init; }
	}
}
