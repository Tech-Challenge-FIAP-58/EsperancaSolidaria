using AutoMapper;
using UserService.Application.Inputs;
using UserService.Application.Web;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Utils;
using UserService.Domain.Models;

namespace UserService.Application.Services
{
    public class UserApplicationService(IPasswordHasher passwordHasher, IUserRepository repository, IMapper mapper) : BaseService, IUserApplicationService
    {
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IUserRepository _userRepository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<IApiResponse<IEnumerable<UserResponseDto>>> GetAll()
        {
            var users = await _userRepository.GetAll();
            var result = _mapper.Map<IEnumerable<UserResponseDto>>(users);
            return Ok(result);
        }

        public async Task<IApiResponse<UserResponseDto?>> GetById(Guid id)
        {
            var user = await _userRepository.GetById(id);

            if (user is null)
                return NotFound<UserResponseDto?>("Usuário não encontrado.");

            var dto = _mapper.Map<UserResponseDto>(user);

            return Ok<UserResponseDto?>(dto);
        }

        public async Task<IApiResponse<UserResponseDto>> Create(UserCreateDto userCreateDto)
        {
            if (await _userRepository.ExistsByEmail(userCreateDto.Email))
                return Conflict<UserResponseDto>("E-mail já cadastrado.");

            if (await _userRepository.ExistsByCpf(userCreateDto.Cpf))
                return Conflict<UserResponseDto>("CPF já cadastrado.");

            var user = _mapper.Map<User>(userCreateDto);

            user.Guid = Guid.NewGuid();
            user.Password = _passwordHasher.Hash(userCreateDto.Password);
            user.Roles = new List<string> { userCreateDto.Role };

            await _userRepository.Create(user);

            var dto = _mapper.Map<UserResponseDto>(user);

            return Created(dto);
        }

        public async Task<IApiResponse<bool>> Update(Guid id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetById(id);
            if (user is null)
                return NotFound<bool>("Usuário não encontrado para atualização.");

            _mapper.Map(dto, user);

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.Password = _passwordHasher.Hash(dto.Password);

            user.UpdatedAt = DateTimeOffset.Now;

            await _userRepository.Update(user);
            return NoContent();
        }

        public async Task<IApiResponse<bool>> Remove(Guid id)
        {
            var user = await _userRepository.GetById(id);

            if (user is null)
                return NotFound<bool>("Usuário não encontrado para remoção.");

            await _userRepository.Remove(user);

            return NoContent();
        }
    }
}
