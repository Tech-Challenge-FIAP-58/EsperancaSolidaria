using System.Net;
using UserService.Application.Web;

namespace UserService.Application.Services
{
    public abstract class BaseService
    {
        public static IApiResponse<T> Ok<T>(T result, string message = "")
            => Build(result, HttpStatusCode.OK, true, message);

        public static IApiResponse<T> Created<T>(T result, string message = "")
            => Build(result, HttpStatusCode.Created, true, message);

        // Para operações sem payload em sucesso (Update/Delete)
        public static IApiResponse<bool> NoContent(string message = "")
            => Build(false, HttpStatusCode.NoContent, true, message);

        public static IApiResponse<T> BadRequest<T>(string message = "")
            => Build(default(T), HttpStatusCode.BadRequest, false, message);

        public static IApiResponse<T> NotFound<T>(string message = "")
            => Build(default(T), HttpStatusCode.NotFound, false, message);

        public static IApiResponse<T> Unauthorized<T>(string message = "")
            => Build(default(T), HttpStatusCode.Unauthorized, false, message);

        public static IApiResponse<T> InternalServerError<T>(string message = "")
            => Build(default(T), HttpStatusCode.InternalServerError, false, message);

        public static IApiResponse<T> Conflict<T>(string message = "")
            => Build(default(T), HttpStatusCode.Conflict, false, message);

        // Fábrica única
        private static IApiResponse<T> Build<T>(T? value, HttpStatusCode code, bool isSuccess, string message)
            => new ApiResponse<T>
            {
                ResultValue = value,
                Message = message,
                StatusCode = code,
                IsSuccess = isSuccess
            };
    }
}
