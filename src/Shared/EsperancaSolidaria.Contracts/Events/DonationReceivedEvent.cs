namespace EsperancaSolidaria.Contracts.Events
{
	// Contrato de evento compartilhado entre os serviços (produtor e consumidores).
	// A identidade do tipo (namespace + nome) é o que o MassTransit usa para rotear,
	// então todos os serviços devem referenciar ESTE projeto — não redeclarar o tipo.
	public class DonationReceivedEvent
	{
		public Guid DonorUserId { get; init; }
		public decimal Amount { get; init; }
	}
}
