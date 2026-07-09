using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UserService.WebApi.HealthChecks
{
    public static class HealthCheckResponse
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        /// <summary>
        /// Corpo JSON com o resultado de cada check. A exceção nunca é serializada:
        /// o endpoint é público e a mensagem carrega detalhe de infraestrutura.
        /// </summary>
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
