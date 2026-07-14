using System.Net;

namespace EsperancaSolidaria.Contracts.Entities.Web
{
    public class ObjectReply<T>(bool isSuccess, HttpStatusCode statusCode, T? value, string? message)
    {
        public bool IsSuccess { get; set; } = isSuccess;
        public string? Message { get; set; } = message;
        public HttpStatusCode StatusCode { get; set; } = statusCode;
        public T? Value { get; set; } = value;
    }
}
