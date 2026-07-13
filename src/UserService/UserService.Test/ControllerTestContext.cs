using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.Test
{
    // Monta um ControllerContext para testar endpoints da camada web isoladamente.
    // Com userId, simula um JWT autenticado (claim "sub"); sem ele, simula ausência de claim.
    internal static class ControllerTestContext
    {
        public static ControllerContext WithUser(Guid? userId = null)
        {
            var identity = userId is null
                ? new ClaimsIdentity()
                : new ClaimsIdentity([new Claim("sub", userId.Value.ToString())], "Test");

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity),
                RequestServices = _services.Value
            };

            return new ControllerContext { HttpContext = httpContext };
        }

        // Problem(...) no ExecuteAsync resolve o ProblemDetailsFactory via RequestServices;
        // AddMvcCore registra a implementação padrão.
        private static readonly Lazy<IServiceProvider> _services = new(() =>
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddMvcCore();
            return services.BuildServiceProvider();
        });
    }
}
