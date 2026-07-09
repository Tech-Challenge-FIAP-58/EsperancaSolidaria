using AutoMapper;
using UserService.Application.Inputs;
using UserService.Application.Services;
using UserService.Application.Web;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Utils;
using UserService.Domain.Models;

namespace UserService.Application.Auth
{
    public class AuthService(IMapper mapper, IPasswordHasher passwordHasher, IUserRepository repo, IJwtTokenGenerator tokenGenerator) : BaseService, IAuthService
    {
        private readonly IUserRepository _repository = repo;
        private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<IApiResponse<string>> Login(LoginDto dto)
        {
            var user = await _repository.FindByEmail(dto.Email);
            if (user is null)
                return Unauthorized<string>("Credenciais inválidas.");

            if (!_passwordHasher.Verify(dto.Password, user.Password))
                return Unauthorized<string>("Credenciais inválidas.");

            var token = _tokenGenerator.Generate(user);
            return Ok(token);
        }

        // Auto-cadastro público: a role é fixada como Doador no servidor (o cliente não a escolhe).
        public async Task<IApiResponse<Guid>> Register(UserRegisterDto dto)
        {
            if (await _repository.ExistsByEmail(dto.Email))
                return Conflict<Guid>("E-mail já cadastrado.");

            if (await _repository.ExistsByCpf(dto.Cpf))
                return Conflict<Guid>("CPF já cadastrado.");

            var entity = _mapper.Map<User>(dto);
            entity.Guid = Guid.NewGuid();
            entity.Password = _passwordHasher.Hash(dto.Password);
            entity.Roles = new List<string> { Roles.Doador };

            await _repository.Create(entity);
            return Created(entity.Guid, "Usuário registrado com sucesso.");
        }
    }
}
