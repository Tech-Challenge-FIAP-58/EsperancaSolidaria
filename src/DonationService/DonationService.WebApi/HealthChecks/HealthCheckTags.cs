namespace DonationService.WebApi.HealthChecks
{
    // A única dependência externa do serviço é o RabbitMQ, cujo health check é
    // registrado automaticamente pelo MassTransit com a tag "masstransit".
    public static class HealthCheckTags
    {
        public const string Dependency = "masstransit";
    }
}
