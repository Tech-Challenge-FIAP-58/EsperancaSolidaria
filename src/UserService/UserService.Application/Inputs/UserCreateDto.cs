using System.ComponentModel.DataAnnotations;
using UserService.Application.Validation;
using UserService.Domain.Models;

namespace UserService.Application.Inputs
{
    /// <summary>
    /// Criação de usuário pela gestão (GestorONG). A role é escolhida explicitamente
    /// pelo chamador — que já é confiável por estar autorizado.
    /// </summary>
    public sealed record class UserCreateDto
    {
        [Required, MinLength(2)]
        public string Name { get; init; } = default!;

        [Required, EmailAddress]
        public string Email { get; init; } = default!;

        [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve ter 11 dígitos numéricos.")]
        [CustomValidation(typeof(ValidaCpf), nameof(ValidaCpf.ValidarCpf))]
        public string Cpf { get; init; } = default!;

        [Required, RegularExpression(@"^(?=.*[A-Z])(?=(?:.*\d){3,})(?=.*[^\w\s])\S{8,}$",
                 ErrorMessage = "Senha deve ter ao menos 8 caracteres, 3 dígitos, 1 maiúscula e 1 caractere especial.")]
        public string Password { get; init; } = default!;

        [Required, AllowedValues(Roles.GestorONG, Roles.Doador,
                 ErrorMessage = "Role inválida. Valores permitidos: GestorONG, Doador.")]
        public string Role { get; init; } = default!;
    }
}
