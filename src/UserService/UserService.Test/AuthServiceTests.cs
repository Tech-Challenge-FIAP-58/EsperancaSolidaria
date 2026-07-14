using System.Net;
using AutoMapper;
using Moq;
using UserService.Application.Auth;
using UserService.Application.Inputs;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Utils;
using UserService.Domain.Models;

namespace UserService.Test
{
    public class AuthServiceTests
    {
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IPasswordHasher> _hasher = new();
        private readonly Mock<IUserRepository> _repo = new();
        private readonly Mock<IJwtTokenGenerator> _tokenGen = new();
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _service = new AuthService(_mapper.Object, _hasher.Object, _repo.Object, _tokenGen.Object);
        }

        private static UserRegisterDto ValidDto() => new()
        {
            Name = "Nome Completo",
            Email = "user@test.com",
            Cpf = "52998224725",
            Password = "Senha@123"
        };

        private static User NewUser(string email) => new()
        {
            Guid = Guid.NewGuid(),
            Email = email,
            Password = "hash",
            Name = "N",
            Cpf = "52998224725",
            Roles = new List<string>()
        };

        [Fact]
        public async Task Login_UserNotFound_ReturnsUnauthorized()
        {
            _repo.Setup(r => r.FindByEmail(It.IsAny<string>())).ReturnsAsync((User?)null);

            var resp = await _service.Login(new LoginDto { Email = "a@b.com", Password = "x" });

            Assert.False(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
            _tokenGen.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            var user = NewUser("a@b.com");
            _repo.Setup(r => r.FindByEmail(user.Email)).ReturnsAsync(user);
            _hasher.Setup(h => h.Verify("x", user.Password)).Returns(false);

            var resp = await _service.Login(new LoginDto { Email = user.Email, Password = "x" });

            Assert.False(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
            _tokenGen.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Login_Valid_ReturnsToken()
        {
            var user = NewUser("a@b.com");
            _repo.Setup(r => r.FindByEmail(user.Email)).ReturnsAsync(user);
            _hasher.Setup(h => h.Verify("x", user.Password)).Returns(true);
            _tokenGen.Setup(t => t.Generate(user)).Returns("jwt-token");

            var resp = await _service.Login(new LoginDto { Email = user.Email, Password = "x" });

            Assert.True(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal("jwt-token", resp.ResultValue);
        }

        [Fact]
        public async Task Register_WhenEmailExists_ReturnsConflict()
        {
            var dto = ValidDto();
            _repo.Setup(r => r.ExistsByEmail(dto.Email)).ReturnsAsync(true);

            var resp = await _service.Register(dto);

            Assert.False(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
            _repo.Verify(r => r.Create(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Register_WhenCpfExists_ReturnsConflict()
        {
            var dto = ValidDto();
            _repo.Setup(r => r.ExistsByEmail(dto.Email)).ReturnsAsync(false);
            _repo.Setup(r => r.ExistsByCpf(dto.Cpf)).ReturnsAsync(true);

            var resp = await _service.Register(dto);

            Assert.False(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
            _repo.Verify(r => r.Create(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Register_Success_AssignsDoadorRole_HashesPassword_Created()
        {
            var dto = ValidDto();
            var mapped = new User { Name = dto.Name, Email = dto.Email, Cpf = dto.Cpf, Password = dto.Password, Roles = new List<string>() };
            User? created = null;

            _repo.Setup(r => r.ExistsByEmail(dto.Email)).ReturnsAsync(false);
            _repo.Setup(r => r.ExistsByCpf(dto.Cpf)).ReturnsAsync(false);
            _mapper.Setup(m => m.Map<User>(dto)).Returns(mapped);
            _hasher.Setup(h => h.Hash(dto.Password)).Returns("hashed");
            _repo.Setup(r => r.Create(It.IsAny<User>())).Callback<User>(u => created = u).ReturnsAsync(true);

            var resp = await _service.Register(dto);

            Assert.True(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            Assert.NotNull(created);
            Assert.NotEqual(Guid.Empty, created!.Guid);
            Assert.Equal("hashed", created.Password);
            Assert.Equal(new[] { Roles.Doador }, created.Roles);
            Assert.Equal(created.Guid, resp.ResultValue);
            _hasher.Verify(h => h.Hash(dto.Password), Times.Once);
        }
    }
}
