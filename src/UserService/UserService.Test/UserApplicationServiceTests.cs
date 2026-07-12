using System.Net;
using AutoMapper;
using Moq;
using UserService.Application.Inputs;
using UserService.Application.Services;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Utils;
using UserService.Domain.Models;

namespace UserService.Test
{
    public class UserApplicationServiceTests
    {
        private readonly Mock<IUserRepository> _repo = new();
        private readonly Mock<IPasswordHasher> _hasher = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly UserApplicationService _service;

        public UserApplicationServiceTests()
        {
            _service = new UserApplicationService(_hasher.Object, _repo.Object, _mapper.Object);
        }

        private static User NewUser(Guid? id = null) => new()
        {
            Guid = id ?? Guid.NewGuid(),
            Name = "User",
            Email = "user@test.com",
            Password = "hash",
            Cpf = "52998224725",
            Roles = new List<string> { Roles.Doador }
        };

        private static UserResponseDto ToDto(User u)
            => new(u.Guid, u.Name, u.Email, u.Cpf, u.Roles, u.CreatedAt);

        [Fact]
        public async Task GetAll_ReturnsMappedUsers_Ok()
        {
            var users = new[] { NewUser() };
            var mapped = new[] { ToDto(users[0]) };
            _repo.Setup(r => r.GetAll()).ReturnsAsync(users);
            _mapper.Setup(m => m.Map<IEnumerable<UserResponseDto>>(It.IsAny<IEnumerable<User>>())).Returns(mapped);

            var resp = await _service.GetAll();

            Assert.True(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal(mapped, resp.ResultValue);
        }

        [Fact]
        public async Task GetById_WhenFound_ReturnsOk()
        {
            var user = NewUser();
            var dto = ToDto(user);
            _repo.Setup(r => r.GetById(user.Guid)).ReturnsAsync(user);
            _mapper.Setup(m => m.Map<UserResponseDto>(user)).Returns(dto);

            var resp = await _service.GetById(user.Guid);

            Assert.True(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal(dto, resp.ResultValue);
        }

        [Fact]
        public async Task GetById_WhenNotFound_ReturnsNotFound()
        {
            _repo.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var resp = await _service.GetById(Guid.NewGuid());

            Assert.False(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
            Assert.Null(resp.ResultValue);
        }

        [Theory]
        [InlineData("GestorONG")]
        [InlineData("Doador")]
        public async Task Create_WithRole_HashesPassword_AssignsGivenRole_ReturnsCreated(string role)
        {
            var dto = new UserCreateDto { Name = "Novo", Email = "novo@test.com", Cpf = "52998224725", Password = "Senha@123", Role = role };
            var mapped = new User { Name = dto.Name, Email = dto.Email, Cpf = dto.Cpf, Password = dto.Password };
            User? created = null;

            _repo.Setup(r => r.ExistsByEmail(dto.Email)).ReturnsAsync(false);
            _repo.Setup(r => r.ExistsByCpf(dto.Cpf)).ReturnsAsync(false);
            _mapper.Setup(m => m.Map<User>(dto)).Returns(mapped);
            _hasher.Setup(h => h.Hash(dto.Password)).Returns("hashed");
            _repo.Setup(r => r.Create(It.IsAny<User>())).Callback<User>(u => created = u).ReturnsAsync(true);
            _mapper.Setup(m => m.Map<UserResponseDto>(It.IsAny<User>()))
                   .Returns(new UserResponseDto(Guid.NewGuid(), dto.Name, dto.Email, dto.Cpf, new List<string> { role }, DateTimeOffset.Now));

            var resp = await _service.Create(dto);

            Assert.True(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            Assert.NotNull(created);
            Assert.NotEqual(Guid.Empty, created!.Guid);
            Assert.Equal("hashed", created.Password);
            Assert.Equal(new[] { role }, created.Roles);
            _hasher.Verify(h => h.Hash(dto.Password), Times.Once);
            _repo.Verify(r => r.Create(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Create_WhenEmailExists_ReturnsConflict()
        {
            var dto = new UserCreateDto { Name = "N", Email = "dup@test.com", Cpf = "52998224725", Password = "Senha@123", Role = Roles.Doador };
            _repo.Setup(r => r.ExistsByEmail(dto.Email)).ReturnsAsync(true);

            var resp = await _service.Create(dto);

            Assert.False(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
            _repo.Verify(r => r.Create(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Create_WhenCpfExists_ReturnsConflict()
        {
            var dto = new UserCreateDto { Name = "N", Email = "novo@test.com", Cpf = "52998224725", Password = "Senha@123", Role = Roles.Doador };
            _repo.Setup(r => r.ExistsByEmail(dto.Email)).ReturnsAsync(false);
            _repo.Setup(r => r.ExistsByCpf(dto.Cpf)).ReturnsAsync(true);

            var resp = await _service.Create(dto);

            Assert.False(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
            _repo.Verify(r => r.Create(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenFound_WithPassword_HashesAndUpdates_NoContent()
        {
            var user = NewUser();
            User? captured = null;
            var dto = new UserUpdateDto { Name = "Novo Nome", Password = "P@ssw0rd123" };

            _repo.Setup(r => r.GetById(user.Guid)).ReturnsAsync(user);
            _mapper.Setup(m => m.Map(dto, user)).Callback<UserUpdateDto, User>((s, d) => { if (s.Name != null) d.Name = s.Name; });
            _hasher.Setup(h => h.Hash(dto.Password!)).Returns("hashed-pass");
            _repo.Setup(r => r.Update(It.IsAny<User>())).Callback<User>(u => captured = u).ReturnsAsync(true);

            var resp = await _service.Update(user.Guid, dto);

            Assert.True(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
            Assert.NotNull(captured);
            Assert.Equal("Novo Nome", captured!.Name);
            Assert.Equal("hashed-pass", captured.Password);
            _hasher.Verify(h => h.Hash(dto.Password!), Times.Once);
            _repo.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenFound_WithoutPassword_DoesNotHash()
        {
            var user = NewUser();
            var dto = new UserUpdateDto { Name = "SoNome" };

            _repo.Setup(r => r.GetById(user.Guid)).ReturnsAsync(user);
            _mapper.Setup(m => m.Map(dto, user));
            _repo.Setup(r => r.Update(It.IsAny<User>())).ReturnsAsync(true);

            var resp = await _service.Update(user.Guid, dto);

            Assert.True(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
            _hasher.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenNotFound_ReturnsNotFound()
        {
            _repo.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var resp = await _service.Update(Guid.NewGuid(), new UserUpdateDto { Name = "X" });

            Assert.False(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task Remove_WhenFound_CallsRemove_NoContent()
        {
            var user = NewUser();
            User? removed = null;
            _repo.Setup(r => r.GetById(user.Guid)).ReturnsAsync(user);
            _repo.Setup(r => r.Remove(It.IsAny<User>())).Callback<User>(u => removed = u).ReturnsAsync(true);

            var resp = await _service.Remove(user.Guid);

            Assert.True(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
            Assert.Equal(user, removed);
            _repo.Verify(r => r.Remove(user), Times.Once);
        }

        [Fact]
        public async Task Remove_WhenNotFound_ReturnsNotFound()
        {
            _repo.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var resp = await _service.Remove(Guid.NewGuid());

            Assert.False(resp.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }
    }
}
