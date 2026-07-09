using System.ComponentModel.DataAnnotations;
using UserService.Application.Validation;

namespace UserService.Test
{
    public class ValidaCpfTests
    {
        [Theory]
        [InlineData("52998224725", true)]      // válido
        [InlineData("39053344705", true)]      // válido
        [InlineData("11144477735", true)]      // válido
        [InlineData("529.982.247-25", true)]   // válido com máscara
        [InlineData("11111111111", false)]     // todos os dígitos iguais
        [InlineData("12345678900", false)]     // dígitos verificadores inválidos
        [InlineData("123", false)]             // tamanho errado
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsCpf_VariousCases_ReturnsExpected(string? cpf, bool expected)
            => Assert.Equal(expected, ValidaCpf.IsCpf(cpf));

        [Fact]
        public void ValidarCpf_ValidCpf_ReturnsSuccess()
        {
            var result = ValidaCpf.ValidarCpf("52998224725", new ValidationContext(new object()));
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void ValidarCpf_InvalidCpf_ReturnsError()
        {
            var result = ValidaCpf.ValidarCpf("00000000000", new ValidationContext(new object()));
            Assert.NotNull(result);
            Assert.Equal("CPF inválido.", result!.ErrorMessage);
        }
    }
}
