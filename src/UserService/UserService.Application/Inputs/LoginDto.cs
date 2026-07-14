using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Inputs
{
    public sealed record class LoginDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
