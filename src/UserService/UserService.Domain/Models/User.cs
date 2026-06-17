namespace UserService.Domain.Models
{
    public class User : EntityBase
    {
        public required string Name { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
        public required string Cpf { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = [];
    }
}
