using UserService.Domain.Events;

namespace UserService.Application.Services
{
	public interface IDonationStatsService
	{
		/// <summary>
		/// Registra a doação recebida na coleção de estatísticas. Idempotente:
		/// reentrega do mesmo DonationId não gera registro duplicado.
		/// </summary>
		Task RegisterDonation(DonationReceivedEvent evt);
	}
}
