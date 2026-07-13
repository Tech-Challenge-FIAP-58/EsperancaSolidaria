namespace UserService.Domain.Interfaces.Repository
{
	public interface IUserStatisticsRepository
	{
		Task<bool> RegisterDonation(Guid messageId, Guid userId, decimal amount);

		Task<decimal> GetTotalByUser(Guid userId);
	}
}
