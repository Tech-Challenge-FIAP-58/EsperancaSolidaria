using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Inputs;
using UserService.Application.Mapping;
using UserService.Domain.Models;

namespace UserService.Test
{
    public class UserProfileTests
    {
        private readonly IMapper _mapper;

        public UserProfileTests()
        {
            var provider = new ServiceCollection()
                .AddAutoMapper(typeof(UserProfile).Assembly)
                .BuildServiceProvider();

            _mapper = provider.GetRequiredService<IMapper>();
        }

        [Fact]
        public void Configuration_IsValid()
            => _mapper.ConfigurationProvider.AssertConfigurationIsValid();

        [Fact]
        public void Map_RegisterDto_To_User_IgnoresGuidAndRoles()
        {
            var dto = new UserRegisterDto { Name = "Nome", Email = "e@t.com", Cpf = "52998224725", Password = "Senha@123" };

            var user = _mapper.Map<User>(dto);

            Assert.Equal("Nome", user.Name);
            Assert.Equal("e@t.com", user.Email);
            Assert.Equal("52998224725", user.Cpf);
            Assert.Equal(Guid.Empty, user.Guid); // gerenciado pela aplicação
            Assert.Empty(user.Roles);            // definido pelo service, não pelo mapper
        }

        [Fact]
        public void Map_User_To_ResponseDto_MapsRoles()
        {
            var user = new User
            {
                Guid = Guid.NewGuid(),
                Name = "N",
                Email = "e@t.com",
                Cpf = "52998224725",
                Password = "secret-hash",
                Roles = new List<string> { Roles.Doador }
            };

            var dto = _mapper.Map<UserResponseDto>(user);

            Assert.Equal(user.Guid, dto.Guid);
            Assert.Equal(user.Email, dto.Email);
            Assert.Contains(Roles.Doador, dto.Roles);
        }

        [Fact]
        public void Map_UpdateDto_To_User_OnlyOverwritesProvidedMembers()
        {
            var user = new User
            {
                Guid = Guid.NewGuid(),
                Name = "Original",
                Email = "orig@t.com",
                Cpf = "52998224725",
                Password = "hash",
                Roles = new List<string> { Roles.Doador }
            };
            var dto = new UserUpdateDto { Name = "Novo Nome" }; // Email/Password nulos

            _mapper.Map(dto, user);

            Assert.Equal("Novo Nome", user.Name);   // atualizado
            Assert.Equal("orig@t.com", user.Email);  // preservado (null não sobrescreve)
            Assert.Equal("hash", user.Password);      // ignorado no mapeamento
            Assert.Equal("52998224725", user.Cpf);    // ignorado no mapeamento
        }
    }
}
