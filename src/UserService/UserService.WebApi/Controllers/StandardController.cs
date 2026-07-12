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
