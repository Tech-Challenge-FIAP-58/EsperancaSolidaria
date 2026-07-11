using UserService.Domain.Models;

namespace UserService.Domain.Interfaces.Repository
{
	public interface IUserStatisticsRepository
	{
		/// <summary>
		/// Registra uma doação. Retorna false quando o documento já existia
		/// (mesmo DonationId vindo de reentrega do bus) — idempotência.
		/// </summary>
		Task<bool> Add(UserDonationStat stat);

		/// <summary>Quantidade de doações do usuário (o "contador" = select).</summary>
		Task<long> CountByUser(Guid userId);

		/// <summary>Total doado pelo usuário.</summary>
		Task<decimal> SumAmountByUser(Guid userId);
	}
}
