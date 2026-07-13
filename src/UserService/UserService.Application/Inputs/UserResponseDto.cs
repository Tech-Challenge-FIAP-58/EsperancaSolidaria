namespace UserService.Application.Inputs
{
    public sealed record UserResponseDto(
        Guid Guid,
        string Name,
        string Email,
        string Cpf,
        IList<string> Roles,
        DateTimeOffset CreatedAt
    );
}
