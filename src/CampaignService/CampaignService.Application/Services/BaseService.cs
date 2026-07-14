using EsperancaSolidaria.Contracts.Entities.Web;
using System.Net;

namespace CampaignService.Application.Services
{
    public abstract class BaseService
    {
        protected ObjectReply<T> Success<T>(T? value = default, string? message = null)
        {
            return new ObjectReply<T>(isSuccess: true, HttpStatusCode.OK, value, message);
        }

        protected ObjectReply<T> BadRequest<T>(string? message = null)
        {
            return new ObjectReply<T>(isSuccess: false, HttpStatusCode.BadRequest, default, message);
        }

        protected ObjectReply<T> NotFound<T>(string? message = null)
        {
            return new ObjectReply<T>(isSuccess: false, HttpStatusCode.NotFound, default, message);
        }

        protected ObjectReply<T> InternalServerError<T>(string? message = null)
        {
            return new ObjectReply<T>(isSuccess: false, HttpStatusCode.InternalServerError, default, message);
        }

        protected ObjectReply<T> Fail<T>(string? message = null)
        {
            return new ObjectReply<T>(isSuccess: false, HttpStatusCode.OK, default, message);
        }

        protected ObjectReply<T> Unauthorized<T>(string? message = null)
        {
            return new ObjectReply<T>(isSuccess: false, HttpStatusCode.Unauthorized, default, message);
        }
    }
}
