using System.Text.RegularExpressions;
using MongoDB.Driver;
using UserService.Domain.Exceptions;

namespace UserService.Infra.Mongo
{
    public static partial class MongoWriteExceptionExtensions
    {
        public static bool IsDuplicateKey(this MongoWriteException exception) =>
            exception.WriteError?.Category == ServerErrorCategory.DuplicateKey;

        /// <summary>
        /// Converte a violação de índice único do Mongo em exceção de domínio,
        /// preservando qual propriedade duplicou.
        /// </summary>
        public static DuplicateEntityException ToDuplicateEntityException(this MongoWriteException exception) =>
            new(ExtractPropertyName(exception.WriteError?.Message), exception);

        // O driver reporta o índice violado no texto do erro:
        //   "E11000 duplicate key error collection: users index: Email_1 dup key: { Email: \"a@b.c\" }"
        // Os índices são criados sem nome explícito, então o Mongo os nomeia "<Campo>_<direção>".
        public static string? ExtractPropertyName(string? writeErrorMessage)
        {
            if (string.IsNullOrEmpty(writeErrorMessage))
                return null;

            var match = IndexNameRegex().Match(writeErrorMessage);

            return match.Success
                ? match.Groups["property"].Value
                : null;
        }

        [GeneratedRegex(@"index:\s*(?<property>.+?)_-?1\b")]
        private static partial Regex IndexNameRegex();
    }
}
