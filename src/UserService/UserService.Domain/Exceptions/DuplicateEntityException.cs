namespace UserService.Domain.Exceptions
{
    /// <summary>
    /// Tentativa de gravar uma entidade que viola uma restrição de unicidade.
    /// Lançada pela camada de persistência, traduzida em HTTP 409 na borda.
    /// </summary>
    public sealed class DuplicateEntityException : Exception
    {
        /// <summary>
        /// Propriedade duplicada (ex.: "Email"). Nula quando a origem não é identificável.
        /// </summary>
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
