using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DonationService.WebApi.HealthChecks
{
    public static class HealthCheckResponse
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public static Task WriteAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = new
            {
                status = report.Status.ToString(),
                totalDurationMs = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    durationMs = entry.Value.Duration.TotalMilliseconds
                })
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(payload, Options));
        }
    }
}
