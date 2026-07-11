namespace UserService.Domain.Exceptions
{
    public sealed class DuplicateEntityException : Exception
    {
        public string? PropertyName { get; }

        public DuplicateEntityException(string? propertyName, Exception? innerException = null)
            : base(BuildMessage(propertyName), innerException)
        {
            PropertyName = propertyName;
        }

        private static string BuildMessage(string? propertyName) =>
            propertyName is null
                ? "Já existe um registro com os dados informados."
                : $"Já existe um registro com o campo '{propertyName}' informado.";
    }
}
