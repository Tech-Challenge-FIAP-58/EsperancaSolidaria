using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Inputs;
using UserService.Application.Services;
using UserService.Domain.Models;

namespace UserService.WebApi.Controllers
{
    [Authorize(Roles = Roles.GestorONG)]
    public class UserController(IUserApplicationService service, ILogger<UserController> logger) : StandardController
    {
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            logger.LogInformation("POST - Criar usuário com role {Role}", dto.Role);
            return await TryMethodAsync(() => service.Create(dto), logger);
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            logger.LogInformation("GET - Listar usuários");
            return await TryMethodAsync(() => service.GetAll(), logger);
        }

        [HttpGet("GetById/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
        {
            logger.LogInformation("GET BY ID - Listar usuário de ID: {Id}", id);
            return await TryMethodAsync(() => service.GetById(id), logger);
        }

        [HttpPut("Update/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
        {
            logger.LogInformation("PUT - Atualizar usuário de ID: {Id}", id);
            return await TryMethodAsync(() => service.Update(id, dto), logger);
        }

        [HttpDelete("Delete/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Remove(Guid id)
        {
            logger.LogInformation("DELETE - Remover usuário de ID: {Id}", id);
            return await TryMethodAsync(() => service.Remove(id), logger);
        }
    }
}
