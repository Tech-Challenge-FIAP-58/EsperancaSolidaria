namespace UserService.WebApi.HealthChecks
{
    // Tag própria em vez da convencional "ready" porque o MassTransit marca
    // o check do bus dele com "ready" ao ser registrado. 
    public static class HealthCheckTags
    {
        public const string Dependency = "dependency";
    }
}
