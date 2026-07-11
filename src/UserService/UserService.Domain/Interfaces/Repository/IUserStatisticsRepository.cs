using UserService.Domain.Models;

namespace UserService.Domain.Interfaces.Repository
{
	public interface IUserStatisticsRepository
	{
		Task<bool> Add(UserDonationStat stat);

		Task<long> CountByUser(Guid userId);

		Task<decimal> SumAmountByUser(Guid userId);
	}
}
