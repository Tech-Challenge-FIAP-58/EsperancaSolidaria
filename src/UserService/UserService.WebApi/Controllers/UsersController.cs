using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Inputs;
using UserService.Application.Services;

namespace UserService.WebApi.Controllers
{
    // Autosserviço da própria conta. A identidade vem do token (não da rota),
    // então o usuário só age sobre si mesmo. Gestão de terceiros fica no UserController.
    [Authorize]
    public class UsersController(IUserApplicationService service, ILogger<UsersController> logger) : StandardController
    {
        [HttpGet("Me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMe()
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            logger.LogInformation("GET - Perfil próprio do usuário {UserId}", userId);
            return await ExecuteAsync(() => service.GetById(userId.Value));
        }

        [HttpPut("Me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateDto dto)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            logger.LogInformation("PUT - Autoatualização do usuário {UserId}", userId);
            return await ExecuteAsync(() => service.Update(userId.Value, dto));
        }

        [HttpDelete("Me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMe()
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            logger.LogInformation("DELETE - Autorremoção do usuário {UserId}", userId);
            return await ExecuteAsync(() => service.Remove(userId.Value));
        }
    }
}
