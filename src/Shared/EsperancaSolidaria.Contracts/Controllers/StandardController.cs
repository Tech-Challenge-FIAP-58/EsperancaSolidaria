using EsperancaSolidaria.Contracts.Entities.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace EsperancaSolidaria.Contracts.Controllers
{
	public class StandardController : ControllerBase
	{
		protected async Task<IActionResult> TryMethodAsync<TResult>(Func<Task<ObjectReply<TResult>>> serviceMethod, ILogger logger)
		{
			try
			{
				var result = await serviceMethod();
				return StatusCode(result.StatusCode.GetHashCode(), result);
			}
			catch (Exception ex)
			{
				logger.LogError(exception: ex, message: ex.Message);
				return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex?.Message);
			}
		}
	}
}
