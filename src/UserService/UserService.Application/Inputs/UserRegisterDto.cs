using System.ComponentModel.DataAnnotations;
using UserService.Application.Validation;

namespace UserService.Application.Inputs
{
    public sealed record class UserRegisterDto
    {
        // Nome Completo
        [Required, MinLength(2)]
        public string Name { get; init; } = default!;

        // Email (único no banco — garantido por índice + verificação no cadastro)
        [Required, EmailAddress]
        public string Email { get; init; } = default!;

        // CPF (formato + dígitos verificadores)
        [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve ter 11 dígitos numéricos.")]
        [CustomValidation(typeof(ValidaCpf), nameof(ValidaCpf.ValidarCpf))]
        public string Cpf { get; init; } = default!;

        // Senha (armazenada com hash BCrypt)
        [Required, RegularExpression(@"^(?=.*[A-Z])(?=(?:.*\d){3,})(?=.*[^\w\s])\S{8,}$",
                 ErrorMessage = "Senha deve ter ao menos 8 caracteres, 3 dígitos, 1 maiúscula e 1 caractere especial.")]
        public string Password { get; init; } = default!;
    }
}
