using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using UserService.Application.Web;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StandardController : ControllerBase
    {
        protected async Task<IActionResult> TryMethodAsync<TResult>(
            Func<Task<IApiResponse<TResult>>> serviceMethod,
            ILogger logger)
        {
            try
            {
                var result = await serviceMethod();

                // 204 não deve ter body
                if (result.StatusCode == HttpStatusCode.NoContent)
                    return StatusCode((int)HttpStatusCode.NoContent);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (ValidationException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.Unauthorized, ex);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.NotFound, ex);
            }
            // Violação de índice único do Mongo (ex.: Email/CPF duplicado)
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                logger.LogError(ex, ex.Message);
                return CreateProblem(HttpStatusCode.Conflict, ex);
            }
            catch (MongoDuplicateKeyException ex)
            {
                logger.LogError(ex, ex.Message);
                return CreateProblem(HttpStatusCode.Conflict, ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado no TryMethodAsync");
                return CreateProblem(HttpStatusCode.InternalServerError, ex);
            }
        }

        private IActionResult CreateProblem(HttpStatusCode code, Exception ex)
        {
            var problem = new ProblemDetails
            {
                Status = (int)code,
                Title = ToDefaultTitle(code),
                Detail = ex.Message
            };

            problem.Extensions["traceId"] = HttpContext?.TraceIdentifier;

            return StatusCode(problem.Status.Value, problem);
        }

        private static string ToDefaultTitle(HttpStatusCode code) => code switch
        {
            HttpStatusCode.BadRequest => "Requisição inválida",
            HttpStatusCode.Unauthorized => "Não autorizado",
            HttpStatusCode.Forbidden => "Proibido",
            HttpStatusCode.NotFound => "Recurso não encontrado",
            HttpStatusCode.Conflict => "Conflito",
            HttpStatusCode.UnprocessableEntity => "Entidade inválida",
            _ => "Erro"
        };
    }
}
