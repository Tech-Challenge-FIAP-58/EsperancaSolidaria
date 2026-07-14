using System.ComponentModel.DataAnnotations;
using UserService.Application.Inputs;

namespace UserService.Test
{
    public class DtoValidationTests
    {
        private static IList<ValidationResult> Validate(object dto)
        {
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void UserRegisterDto_Valid_PassesValidation()
        {
            var dto = new UserRegisterDto
            {
                Name = "Nome Completo",
                Email = "user@test.com",
                Cpf = "52998224725",
                Password = "Senha@123"
            };

            Assert.Empty(Validate(dto));
        }

        [Theory]
        [InlineData("", "user@test.com", "52998224725", "Senha@123")]        // nome vazio
        [InlineData("N", "user@test.com", "52998224725", "Senha@123")]        // nome curto
        [InlineData("Nome", "nao-eh-email", "52998224725", "Senha@123")]      // email inválido
        [InlineData("Nome", "user@test.com", "123", "Senha@123")]             // cpf formato inválido
        [InlineData("Nome", "user@test.com", "12345678900", "Senha@123")]     // cpf dígitos inválidos
        [InlineData("Nome", "user@test.com", "52998224725", "fraca")]         // senha fraca
        public void UserRegisterDto_Invalid_FailsValidation(string name, string email, string cpf, string password)
        {
            var dto = new UserRegisterDto { Name = name, Email = email, Cpf = cpf, Password = password };

            Assert.NotEmpty(Validate(dto));
        }

        [Theory]
        [InlineData("GestorONG")]
        [InlineData("Doador")]
        public void UserCreateDto_ValidRole_PassesValidation(string role)
        {
            var dto = new UserCreateDto { Name = "Nome", Email = "user@test.com", Cpf = "52998224725", Password = "Senha@123", Role = role };

            Assert.Empty(Validate(dto));
        }

        [Theory]
        [InlineData("Admin")]   // role inexistente
        [InlineData("")]        // vazia
        [InlineData("doador")]  // case-sensitive
        public void UserCreateDto_InvalidRole_FailsValidation(string role)
        {
            var dto = new UserCreateDto { Name = "Nome", Email = "user@test.com", Cpf = "52998224725", Password = "Senha@123", Role = role };

            Assert.NotEmpty(Validate(dto));
        }

        [Fact]
        public void LoginDto_InvalidEmail_FailsValidation()
        {
            var dto = new LoginDto { Email = "invalid", Password = "x" };

            Assert.NotEmpty(Validate(dto));
        }

        [Fact]
        public void LoginDto_Valid_PassesValidation()
        {
            var dto = new LoginDto { Email = "user@test.com", Password = "qualquer" };

            Assert.Empty(Validate(dto));
        }
    }
}
