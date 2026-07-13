using UserService.Application.Web;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StandardController : ControllerBase
    {
        // Id do usuário autenticado a partir do JWT. Dependendo do mapeamento de
        // claims do JwtBearer, ele vem como "sub" ou como NameIdentifier.
        protected Guid? GetUserId()
        {
            var id = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : null;
        }

        protected async Task<IActionResult> ExecuteAsync<TResult>(
            Func<Task<IApiResponse<TResult>>> serviceMethod)
        {
            var result = await serviceMethod();
            var status = (int)result.StatusCode;

            if (!result.IsSuccess)
                return Problem(detail: result.Message, statusCode: status);

            // 204 não tem body
            if (result.StatusCode == HttpStatusCode.NoContent)
                return StatusCode(status);

            return StatusCode(status, result.ResultValue);
        }
    }
}
