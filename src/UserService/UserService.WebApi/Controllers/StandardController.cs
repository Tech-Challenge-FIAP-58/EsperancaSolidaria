using UserService.Application.Web;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StandardController : ControllerBase
    {
        protected async Task<IActionResult> ExecuteAsync<TResult>(
            Func<Task<IApiResponse<TResult>>> serviceMethod)
        {
            var result = await serviceMethod();

            // 204 não deve ter body
            if (result.StatusCode == HttpStatusCode.NoContent)
                return StatusCode((int)HttpStatusCode.NoContent);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
