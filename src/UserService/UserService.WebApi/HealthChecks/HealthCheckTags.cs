namespace UserService.WebApi.HealthChecks
{
    public static class HealthCheckTags
    {
        /// <summary>
        /// Dependência externa sem a qual a aplicação não consegue servir tráfego.
        /// Tag própria em vez da convencional "ready" porque o MassTransit marca
        /// o check do bus dele com "ready" ao ser registrado.
        /// </summary>
        public const string Dependency = "dependency";
    }
}
