using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DonationService.Domain.Exceptions;

namespace DonationService.WebApi.Middleware
{
    public sealed class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var status = ToStatusCode(exception);

            if ((int)status >= 500)
            {
                logger.LogError(exception, "Erro inesperado em {Method} {Path}",
                    httpContext.Request.Method, httpContext.Request.Path);
            }
            else
            {
                logger.LogWarning(exception, "{Message}", exception.Message);
            }

            httpContext.Response.StatusCode = (int)status;

            var problem = new ProblemDetails
            {
                Status = (int)status,
                Title = ToDefaultTitle(status),
                Detail = ToDetail(status, exception)
            };

            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problem
            });
        }

        private static HttpStatusCode ToStatusCode(Exception exception) => exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            ArgumentException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            KeyNotFoundException => HttpStatusCode.NotFound,
            DuplicateEntityException => HttpStatusCode.Conflict,

            _ => HttpStatusCode.InternalServerError
        };

        private static string ToDetail(HttpStatusCode status, Exception exception) =>
            status == HttpStatusCode.InternalServerError
                ? "Ocorreu um erro inesperado ao processar a requisição."
                : exception.Message;

        private static string ToDefaultTitle(HttpStatusCode status) => status switch
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
