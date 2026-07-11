namespace UserService.Domain.Events
{
	/// <summary>
	/// Evento publicado pelo serviço de doações quando uma doação é confirmada.
	/// Contrato compartilhado por tipo com o produtor: o namespace e o nome desta
	/// classe devem ser idênticos nos dois lados para o MassTransit rotear a mensagem.
	/// O <see cref="DonationId"/> identifica a transação de doação (único por doação),
	/// não o par usuário/campanha.
	/// </summary>
	public class DonationReceivedEvent
	{
		public Guid DonationId { get; init; }
		public Guid DonorUserId { get; init; }
		public Guid CampaignId { get; init; }
		public decimal Amount { get; init; }
		public DateTimeOffset OccurredAt { get; init; }
	}
}
