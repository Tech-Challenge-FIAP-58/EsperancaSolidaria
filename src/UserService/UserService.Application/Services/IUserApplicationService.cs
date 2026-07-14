using UserService.Application.Inputs;
using UserService.Application.Web;

namespace UserService.Application.Services
{
    public interface IUserApplicationService
    {
        Task<IApiResponse<IEnumerable<UserResponseDto>>> GetAll();
        Task<IApiResponse<UserResponseDto?>> GetById(Guid id);
        Task<IApiResponse<UserResponseDto>> Create(UserCreateDto userCreateDto);
        Task<IApiResponse<bool>> Update(Guid id, UserUpdateDto userUpdateDto);
        Task<IApiResponse<bool>> Remove(Guid id);
    }
}
