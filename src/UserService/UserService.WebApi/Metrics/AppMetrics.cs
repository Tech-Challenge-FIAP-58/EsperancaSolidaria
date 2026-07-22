using Prometheus;

namespace CampaignService.WebApi.Metrics;

public class AppMetrics
{
    public static readonly Counter TotalRequests = Prometheus.Metrics
        .CreateCounter("app_total_requests", "Total de requisições recebidas pela aplicação");
}
