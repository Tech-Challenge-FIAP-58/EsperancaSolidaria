using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Auth;
using UserService.Application.Inputs;

namespace UserService.WebApi.Controllers
{
    public class AuthController(IAuthService service, ILogger<AuthController> logger) : StandardController
    {
        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            logger.LogInformation("POST - Login para o email: {Email}", loginDto.Email);
            return await TryMethodAsync(() => service.Login(loginDto), logger);
        }

        // Auto-cadastro público: cria um Doador.
        [HttpPost("Register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            logger.LogInformation("POST - Registro de doador: {Email}", dto.Email);
            return await TryMethodAsync(() => service.Register(dto), logger);
        }
    }
}
