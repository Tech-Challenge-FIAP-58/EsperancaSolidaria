namespace EsperancaSolidaria.Contracts.Events
{
	// Contrato de evento compartilhado entre os serviços (produtor e consumidores).
	// A identidade do tipo (namespace + nome) é o que o MassTransit usa para rotear,
	// então todos os serviços devem referenciar ESTE projeto — não redeclarar o tipo.
	// Superset dos campos: cada consumidor usa só o que precisa (o UserService ignora
	// CampaignId, o CampaignService ignora o que não lhe interessa) — evita eventos duplicados.
	public class DonationReceivedEvent
	{
		public Guid DonationId { get; init; }
		public Guid DonorUserId { get; init; }
		public Guid CampaignId { get; init; }
		public decimal Amount { get; init; }
		public DateTimeOffset OccurredAt { get; init; }
	}
}
