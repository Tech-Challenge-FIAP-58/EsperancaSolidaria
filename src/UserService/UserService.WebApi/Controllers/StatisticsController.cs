using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Services;

namespace UserService.WebApi.Controllers
{
    [Authorize]
    public class StatisticsController(IDonationStatsService service, ILogger<StatisticsController> logger) : StandardController
    {
        [HttpGet("MyTotal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MyTotal()
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            logger.LogInformation("GET - Total doado do usuário {UserId}", userId);
            return await ExecuteAsync(() => service.GetTotal(userId.Value));
        }
    }
}
